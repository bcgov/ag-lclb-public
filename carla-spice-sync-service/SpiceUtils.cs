﻿
using CarlaSpiceSync.models;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Models;
using Gov.Lclb.Cllb.Interfaces.Spice;
using Hangfire.Console;
using Hangfire.Server;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;
using SpdSync;
using SpdSync.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gov.Lclb.Cllb.SpdSync
{
    public class SpiceUtils
    {
        public ILogger _logger { get; }

        private IConfiguration Configuration { get; }
        private IDynamicsClient _dynamicsClient;
        public ISpiceClient SpiceClient;

        public SpiceUtils(IConfiguration Configuration, ILoggerFactory loggerFactory)
        {
            this.Configuration = Configuration;
            _logger = loggerFactory.CreateLogger(typeof(SpiceUtils));
            _dynamicsClient = DynamicsUtil.SetupDynamics(Configuration);
            SpiceClient = CreateSpiceClient(Configuration);
        }

        public SpiceClient CreateSpiceClient(IConfiguration Configuration)
        {
            string spiceURI = Configuration["SPICE_URI"];
            string token = Configuration["SPICE_JWT_TOKEN"];

            // create JWT credentials
            TokenCredentials credentials = new TokenCredentials(token);

            return new SpiceClient(new Uri(spiceURI), credentials);
        }


        /// <summary>
        /// Hangfire job to send an export to SPD.
        /// </summary>
        public void SendWorkerExportJob(PerformContext hangfireContext)
        {
            hangfireContext.WriteLine("Starting SPD Export Job.");
            _logger.LogError("Starting SPD Export Job.");

            Type type = typeof(MicrosoftDynamicsCRMadoxioSpddatarow);

            string filter = $"adoxio_isexport eq true and adoxio_exporteddate eq null";
            List<MicrosoftDynamicsCRMadoxioSpddatarow> result = null;

            try
            {
                result = _dynamicsClient.Spddatarows.Get(filter: filter).Value.ToList();
            }
            catch (OdataerrorException odee)
            {
                hangfireContext.WriteLine("Error getting SPD data rows");
                hangfireContext.WriteLine("Request:");
                hangfireContext.WriteLine(odee.Request.Content);
                hangfireContext.WriteLine("Response:");
                hangfireContext.WriteLine(odee.Response.Content);

                _logger.LogError("Error getting SPD data rows");
                _logger.LogError("Request:");
                _logger.LogError(odee.Request.Content);
                _logger.LogError("Response:");
                _logger.LogError(odee.Response.Content);
                // fail if we can't get results.
                throw (odee);
            }
            

            if (result != null && result.Count > 0)
            {
                List<Interfaces.Spice.Models.WorkerScreeningRequest> payload = new List<Interfaces.Spice.Models.WorkerScreeningRequest>();

                foreach (var row in result)
                {
                    Guid.TryParse(row.AdoxioLcrbworkerjobid, out Guid workerJobId);
                    var runner = GenerateWorkerScreeningRequest(workerJobId, _logger);
                    runner.Wait();
                    var workerRequest = runner.Result;
                    payload.Add(workerRequest);
                }

                // send to spice.

                var spiceRunner = SpiceClient.ReceiveWorkerScreeningsWithHttpMessagesAsync(payload);
                spiceRunner.Wait();
                var spiceResult = spiceRunner.Result;

                hangfireContext.WriteLine("Response code was");
                hangfireContext.WriteLine(spiceResult.Response.StatusCode.ToString());

                _logger.LogError("Response code was");
                _logger.LogError(spiceResult.Response.StatusCode.ToString());
           }

            hangfireContext.WriteLine("End of SPD Export Job.");
            _logger.LogError("End of SPD Export Job.");
        }

        /// <summary>
        /// Hangfire job to receive an application screening import from SPICE.
        /// </summary>
        public void ReceiveApplicationImportJob(PerformContext hangfireContext, List<ApplicationScreeningResponse> responses)
        {
            hangfireContext.WriteLine("Starting SPICE Import Job for Application Screening.");
            _logger.LogError("Starting SPICE Import Job for Application Screening..");

            ImportApplicationResponses(hangfireContext, responses);

            hangfireContext.WriteLine("Done.");
            _logger.LogError("Done.");
        }

        /// <summary>
        /// Hangfire job to receive an import from SPICE.
        /// </summary>
        public void ReceiveWorkerImportJob(PerformContext hangfireContext, List<WorkerScreeningResponse> responses)
        {
            hangfireContext.WriteLine("Starting SPICE Import Job for Worker Screening.");
            _logger.LogError("Starting SPICE Import Job for Worker Screening.");

            ImportWorkerResponses(hangfireContext, responses);

            hangfireContext.WriteLine("Done.");
            _logger.LogError("Done.");
        }

        /// <summary>
        /// Import responses to Dynamics.
        /// </summary>
        /// <returns></returns>
        private void ImportWorkerResponses(PerformContext hangfireContext, List<WorkerScreeningResponse> responses)
        {
            foreach (WorkerScreeningResponse workerResponse in responses)
            {
                // search for the Personal History Record.
                MicrosoftDynamicsCRMcontact contact = _dynamicsClient.Contacts.Get(filter: $"adoxio_spdjobid eq {workerResponse.RecordIdentifier}").Value[0];
                string historyFilter = $"_adoxio_contactid_value eq {contact.Contactid}";
                MicrosoftDynamicsCRMadoxioPersonalhistorysummary record = _dynamicsClient.Personalhistorysummaries.Get(filter: historyFilter).Value[0];

                if (record != null)
                {
                    UpdateContactConsent(record._adoxioContactidValue);

                    // update the record.
                    MicrosoftDynamicsCRMadoxioPersonalhistorysummary patchRecord = new MicrosoftDynamicsCRMadoxioPersonalhistorysummary()
                    {
                        AdoxioSecuritystatus = WorkerSecurityScreeningResultTranslate.GetTranslatedSecurityStatus(workerResponse.Result),
                        AdoxioCompletedon = DateTimeOffset.Now
                    };

                    try
                    {
                        _dynamicsClient.Personalhistorysummaries.Update(record.AdoxioPersonalhistorysummaryid, patchRecord);
                    }
                    catch (OdataerrorException odee)
                    {
                        hangfireContext.WriteLine("Error updating worker personal history");
                        hangfireContext.WriteLine("Request:");
                        hangfireContext.WriteLine(odee.Request.Content);
                        hangfireContext.WriteLine("Response:");
                        hangfireContext.WriteLine(odee.Response.Content);

                        _logger.LogError("Error updating worker personal history");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                    }
                }
            }
        }

        /// <summary>
        /// Import application responses to Dynamics.
        /// </summary>
        /// <returns></returns>
        private void ImportApplicationResponses(PerformContext hangfireContext, List<ApplicationScreeningResponse> responses)
        {
            foreach (ApplicationScreeningResponse applicationResponse in responses)
            {
                string appFilter = $"adoxio_jobnumber eq '{applicationResponse.RecordIdentifier}'";
                string[] expand = { "adoxio_ApplyingPerson", "adoxio_Applicant", "adoxio_adoxio_application_contact" };
                MicrosoftDynamicsCRMadoxioApplication applicationRecord = _dynamicsClient.Applications.Get(filter: appFilter, expand: expand).Value[0];

                if (applicationRecord != null)
                {
                    var screeningRequest = CreateApplicationScreeningRequest(applicationRecord);
                    var associatesValidated = UpdateConsentExpiry(screeningRequest.Associates);
                    _logger.LogInformation($"Total associates consent expiry updated: {associatesValidated}");

                    // update the date of security status received and the status
                    MicrosoftDynamicsCRMadoxioApplication patchRecord = new MicrosoftDynamicsCRMadoxioApplication()
                    {
                        AdoxioDatereceivedspd = DateTimeOffset.Now,
                        AdoxioChecklistsecurityclearancestatus = ApplicationSecurityScreeningResultTranslate.GetTranslatedSecurityStatus(applicationResponse.Result)
                    };

                    try
                    {
                        if(patchRecord.AdoxioChecklistsecurityclearancestatus != null)
                        {
                            _dynamicsClient.Applications.Update(applicationRecord.AdoxioApplicationid, patchRecord);
                        }
                        else
                        {
                            hangfireContext.WriteLine($"Error updating application - received an invalid status of {applicationResponse.Result}");
                            _logger.LogError($"Error updating application - received an invalid status of {applicationResponse.Result}");
                        }
                    }
                    catch (OdataerrorException odee)
                    {
                        hangfireContext.WriteLine("Error updating application");
                        hangfireContext.WriteLine("Request:");
                        hangfireContext.WriteLine(odee.Request.Content);
                        hangfireContext.WriteLine("Response:");
                        hangfireContext.WriteLine(odee.Response.Content);

                        _logger.LogError("Error updating application");
                        _logger.LogError("Request:");
                        _logger.LogError(odee.Request.Content);
                        _logger.LogError("Response:");
                        _logger.LogError(odee.Response.Content);
                    }
                }
            }
        }

        /// <summary>
        /// Generate an application screening request
        /// </summary>
        /// <returns></returns>
        public Interfaces.Spice.Models.ApplicationScreeningRequest GenerateApplicationScreeningRequest(Guid applicationId)
        {
            string appFilter = "adoxio_applicationid eq " + applicationId;
            string[] expand = { "adoxio_ApplyingPerson", "adoxio_Applicant", "adoxio_adoxio_application_contact" };
            var applications = _dynamicsClient.Applications.Get(filter: appFilter, expand: expand);
            var application = applications.Value[0];

            var screeningRequest = CreateApplicationScreeningRequest(application);

            return screeningRequest;
        }

        /// <summary>
        /// Validates the associate consent.
        /// </summary>
        /// <returns><c>true</c>, if associate consent was validated, <c>false</c> otherwise.</returns>
        /// <param name="associates">Associates.</param>
        private bool ValidateAssociateConsent(List<Interfaces.Spice.Models.LegalEntity> associates)
        {
            try
            {
                /* Validate consent for all associates */
                bool consentValidated = true;
                foreach (var entity in associates)
                {
                    if ((bool)entity.IsIndividual)
                    {
                        var id = entity.Contact.ContactId;
                        var contact = _dynamicsClient.Contacts.Get(filter: "contactid eq " + id).Value[0];
                        if (contact.AdoxioConsentvalidated == null)
                        {
                            consentValidated = false;
                            continue;
                        }
                        ConsentValidated consent = (ConsentValidated)contact.AdoxioConsentvalidated;

                        if (contact.AdoxioConsentvalidated.HasValue && (ConsentValidated)contact.AdoxioConsentvalidated != ConsentValidated.YES)
                        {
                            consentValidated = false;
                        }
                    }
                    else
                    {
                        if (!ValidateAssociateConsent((List<Interfaces.Spice.Models.LegalEntity>)entity.Account.Associates))
                        {
                            consentValidated = false;
                        }
                    }
                }
                return consentValidated;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// Sends the application screening request to spice.
        /// </summary>
        /// <returns>The application screening request success boolean.</returns>
        /// <param name="applicationRequest">Application request.</param>
        public async Task<bool> SendApplicationScreeningRequest(Guid applicationId, Interfaces.Spice.Models.ApplicationScreeningRequest applicationRequest)
        {
            var consentValidated = ValidateAssociateConsent((List<Interfaces.Spice.Models.LegalEntity>)applicationRequest.Associates);

            if (consentValidated)
            {
                List<Interfaces.Spice.Models.ApplicationScreeningRequest> payload = new List<Interfaces.Spice.Models.ApplicationScreeningRequest>
                {
                    applicationRequest
                };

                _logger.LogInformation($"Sending Application {applicationRequest.RecordIdentifier} Screening Request at {DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK")}");
                _logger.LogInformation($"Application has {applicationRequest.Associates.Count} associates");

                var result = await SpiceClient.ReceiveApplicationScreeningsWithHttpMessagesAsync(payload);

                _logger.LogInformation($"Response code was: {result.Response.StatusCode.ToString()}");
                _logger.LogInformation($"Done Send Application {applicationRequest.RecordIdentifier} Screening Request at {DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffffffK")}");

                if(result.Response.StatusCode.ToString() == "OK")
                {
                    MicrosoftDynamicsCRMadoxioApplication update = new MicrosoftDynamicsCRMadoxioApplication()
                    {
                        AdoxioSecurityclearancegenerateddate = DateTimeOffset.Now,
                        AdoxioChecklistsecurityclearancestatus = ApplicationSecurityScreeningResultTranslate.GetTranslatedSecurityStatus("REQUEST SENT")
                    };
                    _dynamicsClient.Applications.Update(applicationId.ToString(), update);
                    return true;
                }
                return false;
            }

            _logger.LogError("Consent not valid for all associates.");
            _dynamicsClient.Applications.Update(applicationId.ToString(), new MicrosoftDynamicsCRMadoxioApplication()
            {
                AdoxioSecurityclearancegenerateddate = DateTimeOffset.Now,
                AdoxioChecklistsecurityclearancestatus = ApplicationSecurityScreeningResultTranslate.GetTranslatedSecurityStatus("CONSENT NOT VALIDATED")
            });
            return false;
        }

        /// <summary>
        /// Sends the worker screening request to spice.
        /// </summary>
        /// <returns>The worker screening request success boolean.</returns>
        /// <param name="workerScreeningRequest">Worker screening request.</param>
        public async Task<bool> SendWorkerScreeningRequest(Gov.Lclb.Cllb.Interfaces.Spice.Models.WorkerScreeningRequest workerScreeningRequest, ILogger logger)
        {
            // send the data
            List<Interfaces.Spice.Models.WorkerScreeningRequest> payload = new List<Interfaces.Spice.Models.WorkerScreeningRequest>
            {
                workerScreeningRequest
            };

            logger.LogInformation($"Sending Worker Screening Request");

            var result = await SpiceClient.ReceiveWorkerScreeningsWithHttpMessagesAsync(payload);

            logger.LogInformation($"Response code was: {result.Response.StatusCode.ToString()}");
            logger.LogInformation($"Done Send Worker Screening Request");

            return result.Response.StatusCode.ToString() == "OK";
        }

        public async Task<Interfaces.Spice.Models.WorkerScreeningRequest> GenerateWorkerScreeningRequest(Guid WorkerId, ILogger logger)
        {
            // Query Dynamics for application data
            var worker = await _dynamicsClient.GetWorkerByIdWithChildren(WorkerId.ToString());

            /* Create application */
            Interfaces.Spice.Models.WorkerScreeningRequest request = new Interfaces.Spice.Models.WorkerScreeningRequest()
            {
                Name = worker.AdoxioName,
                BirthDate = worker.AdoxioDateofbirth,
                SelfDisclosure = ((GeneralYesNo)worker.AdoxioSelfdisclosure).ToString(),
                Gender = ((AdoxioGenderCode)worker.AdoxioGendercode).ToString(),
                Birthplace = worker.AdoxioBirthplace,
                BcIdCardNumber = worker.AdoxioBcidcardnumber,
                DriversLicence = worker.AdoxioDriverslicencenumber
            };

            /* Add applicant details */
            if (worker.AdoxioContactId != null)
            {
                request.RecordIdentifier = worker.AdoxioContactId.AdoxioSpdjobid.ToString();
                request.Contact = new Interfaces.Spice.Models.Contact()
                {
                    SpdJobId = worker.AdoxioContactId.AdoxioSpdjobid.ToString(),
                    ContactId = worker.AdoxioContactId.Contactid,
                    FirstName = worker.AdoxioContactId.Firstname,
                    LastName = worker.AdoxioContactId.Lastname,
                    MiddleName = worker.AdoxioContactId.Middlename,
                    Email = worker.AdoxioContactId.Emailaddress1,
                    PhoneNumber = worker.AdoxioContactId.Telephone1,
                    Address = new Interfaces.Spice.Models.Address()
                    {
                        AddressStreet1 = worker.AdoxioContactId.Address1Line1,
                        AddressStreet2 = worker.AdoxioContactId.Address1Line2,
                        AddressStreet3 = worker.AdoxioContactId.Address1Line3,
                        City = worker.AdoxioContactId.Address1City,
                        StateProvince = worker.AdoxioContactId.Address1Stateorprovince,
                        Postal = (CarlaSpiceSync.Validation.ValidatePostalCode(worker.AdoxioContactId.Address1Postalcode)) ? worker.AdoxioContactId.Address1Postalcode : null,
                        Country = worker.AdoxioContactId.Address1Country
                    }
                };

                request.Address = new Interfaces.Spice.Models.Address()
                {
                    AddressStreet1 = worker.AdoxioContactId.Address1Line1,
                    AddressStreet2 = worker.AdoxioContactId.Address1Line2,
                    AddressStreet3 = worker.AdoxioContactId.Address1Line3,
                    City = worker.AdoxioContactId.Address1City,
                    StateProvince = worker.AdoxioContactId.Address1Stateorprovince,
                    Postal = (CarlaSpiceSync.Validation.ValidatePostalCode(worker.AdoxioContactId.Address1Postalcode)) ? worker.AdoxioContactId.Address1Postalcode : null,
                    Country = worker.AdoxioContactId.Address1Country
                };
            }

            if (worker.AdoxioWorkerAliases != null)
            {
                request.Aliases = new List<Interfaces.Spice.Models.Alias>();
                foreach (var alias in worker.AdoxioWorkerAliases)
                {
                    Interfaces.Spice.Models.Alias newAlias = new Interfaces.Spice.Models.Alias()
                    {
                        GivenName = alias.AdoxioLastname,
                        Surname = alias.AdoxioFirstname,
                        SecondName = alias.AdoxioMiddlename,  
                    };
                    request.Aliases.Add(newAlias);
                }
            }

            if (worker.AdoxioWorkerPreviousaddresses != null)
            {
                request.PreviousAddresses = new List<Interfaces.Spice.Models.Address>();
                foreach (var address in worker.AdoxioWorkerPreviousaddresses)
                {
                    Interfaces.Spice.Models.Address newAddress = new Interfaces.Spice.Models.Address()
                    {
                        AddressStreet1 = address.AdoxioStreetaddress,
                        City = address.AdoxioCity,
                        StateProvince = address.AdoxioProvstate,
                        Postal = address.AdoxioPostalcode,
                        Country = address.AdoxioCountry,
                        ToDate = address.AdoxioTodate,
                        FromDate = address.AdoxioFromdate
                    };
                    request.PreviousAddresses.Add(newAddress);
                }
            }

            logger.LogInformation("Finished building Model");
            return request;
        }

        private Gov.Lclb.Cllb.Interfaces.Spice.Models.ApplicationScreeningRequest CreateApplicationScreeningRequest(MicrosoftDynamicsCRMadoxioApplication application)
        {
            var screeningRequest = new Gov.Lclb.Cllb.Interfaces.Spice.Models.ApplicationScreeningRequest()
            {
                Name = application.AdoxioName,
                RecordIdentifier = application.AdoxioJobnumber,
                UrgentPriority = false,
                ApplicantType = Gov.Lclb.Cllb.Interfaces.Spice.Models.SpiceApplicantType.Cannabis,
                DateSent = DateTimeOffset.Now,
                BusinessNumber = application.AdoxioApplicant.Accountnumber,
                ApplicantName = application.AdoxioNameofapplicant,
                BusinessAddress = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Address()
                {
                    AddressStreet1 = application.AdoxioApplicant.Address1Line1,
                    City = application.AdoxioApplicant.Address1City,
                    StateProvince = application.AdoxioApplicant.Address1Stateorprovince,
                    Postal = (CarlaSpiceSync.Validation.ValidatePostalCode(application.AdoxioApplicant.Address1Postalcode)) ? application.AdoxioApplicant.Address1Postalcode : null,
                    Country = application.AdoxioApplicant.Address1Country
                },
                ContactPerson = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Contact()
                {
                    FirstName = application.AdoxioContactpersonfirstname,
                    LastName = application.AdoxioContactpersonlastname,
                    MiddleName = application.AdoxioContactmiddlename,
                    Email = application.AdoxioEmail,
                    PhoneNumber = application.AdoxioContactpersonphone
                }
            };
            if (application.AdoxioApplyingPerson != null)
            {
                string companyName = null;
                if (application.AdoxioApplyingPerson._parentcustomeridValue != null) {
                    MicrosoftDynamicsCRMaccount company = _dynamicsClient.Accounts.Get(filter: "accountid eq " + application.AdoxioApplyingPerson._parentcustomeridValue).Value[0];
                    companyName = company.Name;
                }
                screeningRequest.ApplyingPerson = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Contact()
                {
                    SpdJobId = application.AdoxioApplyingPerson.AdoxioSpdjobid.ToString(),
                    ContactId = application.AdoxioApplyingPerson.Contactid,
                    FirstName = application.AdoxioApplyingPerson.Firstname,
                    CompanyName = companyName,
                    MiddleName = application.AdoxioApplyingPerson.Middlename,
                    LastName = application.AdoxioApplyingPerson.Lastname,
                    Email = application.AdoxioApplyingPerson.Emailaddress1
                };
            }
            /* Add applicant details */
            if (application.AdoxioApplicant != null)
            {
                screeningRequest.ApplicantAccount = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Account()
                {
                    AccountId = application.AdoxioApplicant.Accountid,
                    Name = application.AdoxioApplicant.Name,
                    BcIncorporationNumber = application.AdoxioApplicant.AdoxioBcincorporationnumber
                };
            }

            /* Add establishment */
            if (application.AdoxioEstablishment != null)
            {
                screeningRequest.Establishment = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Establishment()
                {
                    Name = application.AdoxioEstablishmentpropsedname,
                    PrimaryPhone = application.AdoxioEstablishmentphone,
                    PrimaryEmail = application.AdoxioEstablishmentemail,
                    ParcelId = application.AdoxioEstablishmentparcelid,
                    Address = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Address()
                    {
                        AddressStreet1 = application.AdoxioEstablishmentaddressstreet,
                        City = application.AdoxioEstablishmentaddresscity,
                        StateProvince = "BC",
                        Postal = (CarlaSpiceSync.Validation.ValidatePostalCode(application.AdoxioEstablishmentaddresspostalcode)) ? application.AdoxioEstablishmentaddresspostalcode : null,
                        Country = "Canada"
                    }
                };
            }

            /* Add key personnel and deemed associates */
            screeningRequest.Associates = new List<Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity>();
            string keypersonnelfilter = "(_adoxio_relatedapplication_value eq " + application.AdoxioApplicationid + " and adoxio_iskeypersonnel eq true and adoxio_isindividual eq 1)";
            string deemedassociatefilter = "(_adoxio_relatedapplication_value eq " + application.AdoxioApplicationid + " and adoxio_isdeemedassociate eq true and adoxio_isindividual eq 1)";
            string[] expand = { "adoxio_Contact" };
            var associates = _dynamicsClient.Legalentities.Get(filter: keypersonnelfilter + " or " + deemedassociatefilter, expand: expand).Value;
            if (associates != null)
            {
                foreach (var legalEntity in associates)
                {
                    Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity person = CreateAssociate(legalEntity);
                    screeningRequest.Associates.Add(person);
                }
            }

            /* Add associates from account */
            var moreAssociates = CreateApplicationAssociatesScreeningRequest(application._adoxioApplicantValue, screeningRequest.Associates);
            screeningRequest.Associates = screeningRequest.Associates.Concat(moreAssociates).ToList();

            return screeningRequest;
        }

        private List<Interfaces.Spice.Models.LegalEntity> CreateApplicationAssociatesScreeningRequest(string accountId, IList<Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity> foundAssociates)
        {
            List<Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity> newAssociates = new List<Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity>();
            string applicationfilter = "_adoxio_account_value eq " + accountId + " and _adoxio_profilename_value ne " + accountId;
            foreach (var assoc in foundAssociates)
            {
                if (accountId != assoc.EntityId)
                {
                    applicationfilter += " and adoxio_legalentityid ne " + assoc.EntityId;
                }
            }
            string[] expand = { "adoxio_Contact", "adoxio_Account"};

            var legalEntities = _dynamicsClient.Legalentities.Get(filter: applicationfilter, expand: expand).Value;
            if (legalEntities != null)
            {
                foreach (var legalEntity in legalEntities)
                {
                    Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity associate = CreateAssociate(legalEntity);
                    newAssociates.Add(associate);
                }
            }
            var newFoundAssociates = new List<Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity>(foundAssociates);
            newFoundAssociates.AddRange(newAssociates);
            foreach (var assoc in newAssociates.ToList())
            {
                if (assoc.IsIndividual != true)
                {
                    var moreAssociates = CreateApplicationAssociatesScreeningRequest(assoc.Account.AccountId, newFoundAssociates);
                    assoc.Account.Associates = moreAssociates;
                }
            }
            return newAssociates;
        }

        private Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity CreateAssociate(MicrosoftDynamicsCRMadoxioLegalentity legalEntity)
        {
            Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity associate = new Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity()
            {
                EntityId = legalEntity.AdoxioLegalentityid,
                Name = legalEntity.AdoxioName,
                InterestPercentage = (double?)legalEntity.AdoxioInterestpercentage,
                AppointmentDate = legalEntity.AdoxioDateofappointment,
                NumberVotingShares = legalEntity.AdoxioCommonvotingshares,
                Title = legalEntity.AdoxioJobtitle,
                Positions = GetLegalEntityPositions(legalEntity),
                PreviousAddresses = new List<Gov.Lclb.Cllb.Interfaces.Spice.Models.Address>(),
                Aliases = new List<Gov.Lclb.Cllb.Interfaces.Spice.Models.Alias>()
            };
            
            if (legalEntity.AdoxioIsindividual != null && legalEntity.AdoxioIsindividual == 1 && legalEntity.AdoxioContact != null)
            {
                associate.IsIndividual = true;
                associate.TiedHouse = legalEntity.AdoxioContact.AdoxioSelfdeclaredtiedhouse == 1;
                associate.Contact = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Contact()
                {
                    SpdJobId = legalEntity.AdoxioContact.AdoxioSpdjobid.ToString(),
                    ContactId = legalEntity.AdoxioContact.Contactid,
                    FirstName = legalEntity.AdoxioContact.Firstname,
                    LastName = legalEntity.AdoxioContact.Lastname,
                    MiddleName = legalEntity.AdoxioContact.Middlename,
                    Email = legalEntity.AdoxioContact.Emailaddress1,
                    PhoneNumber = legalEntity.AdoxioContact.Telephone1,
                    SelfDisclosure = (legalEntity.AdoxioContact.AdoxioSelfdisclosure == null) ? null : ((GeneralYesNo)legalEntity.AdoxioContact.AdoxioSelfdisclosure).ToString(),
                    Gender = (legalEntity.AdoxioContact.AdoxioGendercode == null) ? null : ((AdoxioGenderCode)legalEntity.AdoxioContact.AdoxioGendercode).ToString(),
                    Birthplace = legalEntity.AdoxioContact.AdoxioBirthplace,
                    BirthDate = legalEntity.AdoxioContact.Birthdate,
                    BcIdCardNumber = legalEntity.AdoxioContact.AdoxioBcidcardnumber,
                    DriversLicenceNumber = legalEntity.AdoxioContact.AdoxioDriverslicencenumber,
                    Address = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Address()
                    {
                        AddressStreet1 = legalEntity.AdoxioContact.Address1Line1,
                        AddressStreet2 = legalEntity.AdoxioContact.Address1Line2,
                        AddressStreet3 = legalEntity.AdoxioContact.Address1Line3,
                        City = legalEntity.AdoxioContact.Address1City,
                        StateProvince = legalEntity.AdoxioContact.Address1Stateorprovince,
                        Postal = (CarlaSpiceSync.Validation.ValidatePostalCode(legalEntity.AdoxioContact.Address1Postalcode)) ? legalEntity.AdoxioContact.Address1Postalcode : null,
                        Country = legalEntity.AdoxioContact.Address1Country
                    }
                };

                /* Add previous addresses */
                var previousAddresses = _dynamicsClient.Previousaddresses.Get(filter: "_adoxio_contactid_value eq " + legalEntity.AdoxioContact.Contactid).Value;
                foreach (var address in previousAddresses)
                {
                    var newAddress = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Address()
                    {
                        AddressStreet1 = address.AdoxioStreetaddress,
                        City = address.AdoxioCity,
                        StateProvince = address.AdoxioProvstate,
                        Postal = (CarlaSpiceSync.Validation.ValidatePostalCode(address.AdoxioPostalcode)) ? address.AdoxioPostalcode : null,
                        Country = address.AdoxioCountry,
                        ToDate = address.AdoxioTodate,
                        FromDate = address.AdoxioFromdate
                    };
                    associate.PreviousAddresses.Add(newAddress);
                }

                /* Add aliases */
                var aliases = _dynamicsClient.Aliases.Get(filter: "_adoxio_contactid_value eq " + legalEntity.AdoxioContact.Contactid).Value;
                foreach (var alias in aliases)
                {
                    associate.Aliases.Add(new Gov.Lclb.Cllb.Interfaces.Spice.Models.Alias()
                    {
                        GivenName = alias.AdoxioFirstname,
                        Surname = alias.AdoxioLastname,
                        SecondName = alias.AdoxioMiddlename
                    });
                }
            }
            else
            {
                // Attach the account
                if (legalEntity._adoxioShareholderaccountidValue != null)
                {
                    var account = _dynamicsClient.Accounts.Get(filter: "accountid eq " + legalEntity._adoxioShareholderaccountidValue).Value;
                    associate.Account = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Account()
                    {
                        AccountId = account[0].Accountid,
                        Name = account[0].Name,
                        BcIncorporationNumber = account[0].AdoxioBcincorporationnumber,
                        BusinessNumber = account[0].Accountnumber,
                        Associates = new List<Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity>()
                    };
                }
                else if (legalEntity.AdoxioAccount != null)
                {
                    associate.Account = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Account()
                    {
                        AccountId = legalEntity.AdoxioAccount.Accountid,
                        Name = legalEntity.AdoxioAccount.Name,
                        BcIncorporationNumber = legalEntity.AdoxioAccount.AdoxioBcincorporationnumber,
                        BusinessNumber = legalEntity.AdoxioAccount.Accountnumber,
                        Associates = new List<Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity>()
                    };
                }
                else
                {
                    _logger.LogError("Failed to find a shareholder account found");
                    associate.Account = new Gov.Lclb.Cllb.Interfaces.Spice.Models.Account();
                }
                associate.IsIndividual = false;
            }
            return associate;
        }
        public List<string> GetLegalEntityPositions(MicrosoftDynamicsCRMadoxioLegalentity legalEntity)
        {
            List<string> positions = new List<string>();
            if (legalEntity.AdoxioIsdirector != null && (bool)legalEntity.AdoxioIsdirector)
            {
                positions.Add("director");
            }
            if (legalEntity.AdoxioIsofficer != null && (bool)legalEntity.AdoxioIsofficer)
            {
                positions.Add("officer");
            }
            if (legalEntity.AdoxioIsseniormanagement != null && (bool)legalEntity.AdoxioIsseniormanagement)
            {
                positions.Add("senior manager");
            }
            if (legalEntity.AdoxioIskeypersonnel != null && (bool)legalEntity.AdoxioIskeypersonnel)
            {
                positions.Add("key personnel");
            }
            if (legalEntity.AdoxioIsshareholder != null && (bool)legalEntity.AdoxioIsshareholder)
            {
                positions.Add("shareholder");
            }
            if (legalEntity.AdoxioIsowner != null && (bool)legalEntity.AdoxioIsowner)
            {
                positions.Add("owner");
            }
            if (legalEntity.AdoxioIstrustee != null && (bool)legalEntity.AdoxioIstrustee)
            {
                positions.Add("trustee");
            }
            if (legalEntity.AdoxioIsdeemedassociate !=null && (bool)legalEntity.AdoxioIsdeemedassociate)
            {
                positions.Add("deemed associate");
            }
            if (legalEntity.AdoxioIspartner != null && (bool)legalEntity.AdoxioIspartner)
            {
                positions.Add("partner");
            }
            return positions;
        }

        public int UpdateConsentExpiry(IList<Gov.Lclb.Cllb.Interfaces.Spice.Models.LegalEntity> associates)
        {
            var i = 0;
            foreach(var associate in associates)
            {
                _logger.LogError(associate.Name);
                if((bool) associate.IsIndividual)
                {
                    UpdateContactConsent(associate.Contact.ContactId);
                    i += 1;
                }
                else
                {
                    i += UpdateConsentExpiry(associate.Account.Associates);
                }
            }
            return i;
        }

        public void UpdateContactConsent(string ContactId)
        {
            // update consent validated to yes
            MicrosoftDynamicsCRMcontact contact = new MicrosoftDynamicsCRMcontact()
            {
                AdoxioConsentvalidated = 845280000,
                AdoxioConsentvalidatedexpirydate = DateTimeOffset.Now.AddMonths(3)
            };
            _dynamicsClient.Contacts.Update(ContactId, contact);
        }

        public async Task SendFoundWorkers(PerformContext hangfireContext)
        {

        }

        public async Task SendFoundApplications(PerformContext hangfireContext)
        {
            string sendFilter = "adoxio_appchecklistsentspd eq 1 and adoxio_checklistsecurityclearancestatus eq " + ApplicationSecurityScreeningResultTranslate.GetTranslatedSecurityStatus("REQUEST NOT SENT");
            var applications = _dynamicsClient.Applications.Get(filter: sendFilter).Value;
            _logger.LogError($"Found {applications.Count} applications to send to SPD.");
            hangfireContext.WriteLine($"Found {applications.Count} applications to send to SPD.");

            foreach (var application in applications)
            {
                Guid.TryParse(application.AdoxioApplicationid, out Guid applicationId);
                var screeningRequest = GenerateApplicationScreeningRequest(applicationId);
                var response = await SendApplicationScreeningRequest(applicationId, screeningRequest);
                if (response)
                {
                    hangfireContext.WriteLine($"Successfully sent application {application.AdoxioApplicationid} to SPD");
                    _logger.LogError($"Successfully sent application {application.AdoxioApplicationid} to SPD");
                }
                else
                {
                    hangfireContext.WriteLine($"Failed to send application {application.AdoxioApplicationid} to SPD");
                    _logger.LogError($"Failed to send application {application.AdoxioApplicationid} to SPD");
                }
            }
        }
    }
}