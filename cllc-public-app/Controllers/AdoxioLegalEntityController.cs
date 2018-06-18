﻿using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
using Gov.Lclb.Cllb.Interfaces;
using Gov.Lclb.Cllb.Interfaces.Microsoft.Dynamics.CRM;
using Gov.Lclb.Cllb.Public.Authentication;
using Gov.Lclb.Cllb.Public.Models;
using Gov.Lclb.Cllb.Public.Utility;
using Gov.Lclb.Cllb.Public.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.OData.Client;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;

namespace Gov.Lclb.Cllb.Public.Controllers
{
    [Route("api/[controller]")]
    public class AdoxioLegalEntityController : Controller
    {
        private readonly IConfiguration Configuration;
        private readonly Interfaces.Microsoft.Dynamics.CRM.System _system;
        private readonly IDistributedCache _distributedCache;
        private readonly SharePointFileManager _sharePointFileManager;
        private readonly string _encryptionKey;
        private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ILogger _logger;        

		public AdoxioLegalEntityController(Interfaces.Microsoft.Dynamics.CRM.System context, IConfiguration configuration, IDistributedCache distributedCache, SharePointFileManager sharePointFileManager, IHttpContextAccessor httpContextAccessor, ILoggerFactory loggerFactory)
        {
            Configuration = configuration;
            this._system = context;
            this._distributedCache = null; // distributedCache;
            this._sharePointFileManager = sharePointFileManager;
            this._encryptionKey = Configuration["ENCRYPTION_KEY"];
            this._httpContextAccessor = httpContextAccessor;
			_logger = loggerFactory.CreateLogger(typeof(AdoxioLegalEntityController));                    
        }

        /// <summary>
        /// Get all Dynamics Legal Entities
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpGet()]
        public async Task<JsonResult> GetDynamicsLegalEntities()
        {
            List<ViewModels.AdoxioLegalEntity> result = new List<AdoxioLegalEntity>();
            IEnumerable<Adoxio_legalentity> legalEntities = null;
            String accountfilter = null;

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // set account filter
            accountfilter = "_adoxio_account_value eq " + userSettings.AccountId;
			_logger.LogError("Account filter = " + accountfilter);

            legalEntities = await _system.Adoxio_legalentities
                        .AddQueryOption("$filter", accountfilter)
                        .ExecuteAsync();

            foreach (var legalEntity in legalEntities)
            {
                result.Add(legalEntity.ToViewModel());
            }

            return Json(result);
        }

        /// <summary>
        /// Get all Legal Entities where the position matches the parameter received
        /// By default, the account linked to the current user is used
        /// </summary>
        /// <param name="positionType"></param>
        /// <returns></returns>
        [HttpGet()]
        [Route("position/{positionType}")]
        public async Task<JsonResult> GetDynamicsLegalEntitiesByPosition(string positionType)
        {
            List<ViewModels.AdoxioLegalEntity> result = new List<AdoxioLegalEntity>();
            IEnumerable<Adoxio_legalentity> legalEntities = null;
            String positionFilter = null;
            String accountfilter = null;
            String filter = null;

            // get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);

            // set account filter
            accountfilter = "_adoxio_account_value eq " + userSettings.AccountId;
            filter = accountfilter;

            try
            {
                if (positionType == null)
                {
					_logger.LogError("Account filter = " + filter);
                    legalEntities = await _system.Adoxio_legalentities
                        .AddQueryOption("$filter", filter)
                        .ExecuteAsync();

                }
                else
                {
                    positionFilter = Models.Adoxio_LegalEntityExtensions.GetPositionFilter(positionType);
					filter = accountfilter + " and " + positionFilter;
                    //filter = positionFilter;

                    // Execute query if filter is valid
                    if (filter != null)
                    {
						_logger.LogError("Account filter = " + filter);
                        legalEntities = await _system.Adoxio_legalentities
                        .AddQueryOption("$filter", filter)
                        .ExecuteAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                throw;
            }


            if (legalEntities != null)
            {
                foreach (var legalEntity in legalEntities)
                {
                    result.Add(legalEntity.ToViewModel());
                }
            }

            return Json(result);
        }

        /// <summary>
        /// Get a specific legal entity
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDynamicsLegalEntity(string id)
        {
            ViewModels.AdoxioLegalEntity result = null;
            // query the Dynamics system to get the legal entity record.

            Guid? adoxio_legalentityid = new Guid(id);
            Adoxio_legalentity legalEntity = null;
            if (adoxio_legalentityid != null)
            {
                try
                {
                    legalEntity = await _system.Adoxio_legalentities.ByKey(adoxio_legalentityid: adoxio_legalentityid).GetValueAsync();
                    result = legalEntity.ToViewModel();
                }
                catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
                {
                    Console.WriteLine(dsqe.Message);
                    Console.WriteLine(dsqe.StackTrace);
                    return new NotFoundResult();
                }
            }

            return Json(result);
        }

        // get a list of files associated with this legal entity.
        [HttpGet("{id}/attachments")]
        public async Task<IActionResult> GetFiles([FromRoute] string id)
        {
            List<ViewModels.FileSystemItem> result = new List<ViewModels.FileSystemItem>();
            // get the LegalEntity.
            Adoxio_legalentity legalEntity = null;

            if (id != null)
            {
                Guid adoxio_legalentityid = new Guid(id);
                try
                {
                    legalEntity = await _system.Adoxio_legalentities.ByKey(adoxio_legalentityid: adoxio_legalentityid).GetValueAsync();
                    string sanitized = legalEntity.Adoxio_name.Replace(" ", "_");
                    string folder_name = "LegalEntity_Files_" + sanitized;
                    // Get the folder contents for this Legal Entity.
                    List<MS.FileServices.FileSystemItem> items = await _sharePointFileManager.GetFilesInFolder("Documents", folder_name);
                    foreach (MS.FileServices.FileSystemItem item in items)
                    {
                        result.Add(item.ToViewModel());
                    }
                }
                catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
                {
                    return new NotFoundResult();
                }
            }

            return Json(result);
        }

        [HttpPost("{accountId}/attachments")]
        public async Task<IActionResult> UploadFile([FromRoute] string accountId, [FromForm]IFormFile file, [FromForm] string documentType)
        {
            ViewModels.FileSystemItem result = null;
            // get the LegalEntity.
            // Adoxio_legalentity legalEntity = null;

            if (accountId != null)
            {
                // Guid adoxio_legalentityid = new Guid(accountId);
                try
                {
                    var accountGUID = new Guid(accountId);
                     var account = await _system.Accounts.ByKey(accountid: accountGUID).GetValueAsync();

                    string fileName = FileSystemItemExtensions.CombineNameDocumentType(file.FileName, documentType);
                    var accountIdCleaned = account.Accountid.ToString().ToUpper().Replace("-", "");
                    string folderName = $"{account.Name}_{accountIdCleaned}";

                    await _sharePointFileManager.AddFile(folderName, fileName, file.OpenReadStream(), file.ContentType);
                }
                catch (Exception dsqe)
                {
                    return new NotFoundResult();
                }
            }
            return Json(result);
        }

        [HttpGet("{id}/attachments/{fileId}")]
        public async Task<IActionResult> DownloadFile([FromRoute] string id, [FromRoute] string fileId)
        {
            // get the file.
            if (fileId == null)
            {
                return BadRequest();
            }
            else
            {
                _sharePointFileManager.GetFileById(fileId);
            }
            string filename = "";
            byte[] fileContents = new byte[10];
            return new FileContentResult(fileContents, "application/octet-stream")
            {
                FileDownloadName = filename
            };
        }

        /// <summary>
        /// Create a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> CreateDynamicsLegalEntity([FromBody] ViewModels.AdoxioLegalEntity item)
        {
			// create a DataServiceCollection to add the record
            DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Adoxio_legalentity> LegalEntityCollection = new DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Adoxio_legalentity>(_system);

            // create a new legal entity.
            Interfaces.Microsoft.Dynamics.CRM.Adoxio_legalentity adoxioLegalEntity = new Interfaces.Microsoft.Dynamics.CRM.Adoxio_legalentity();

			// add Dynamics LegalEntity to LegalEntity Collection
            LegalEntityCollection.Add(adoxioLegalEntity);

			// get the current user.
            string temp = _httpContextAccessor.HttpContext.Session.GetString("UserSettings");
            UserSettings userSettings = JsonConvert.DeserializeObject<UserSettings>(temp);
			var userAccount = await _system.GetAccountById(_distributedCache, Guid.Parse(userSettings.AccountId));
			//_system.UpdateObject(userAccount);

            // copy received values to Dynamics LegalEntity
            // !!!! Values must be copied after adding to the collection, otherwise the entity will be created without the values assigned !!!!
            adoxioLegalEntity.CopyValues(item, _system);
			adoxioLegalEntity.Adoxio_Account = userAccount;

            // PostOnlySetProperties is used so that settings such as owner will get set properly by the dynamics server.
			DataServiceResponse dsr = _system.SaveChangesSynchronous(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithSingleChangeset);            
            foreach (OperationResponse result in dsr)
            {
                if (result.StatusCode == 500) // error
                {
                    return StatusCode(500, result.Error.Message);
                }
            }
            // get the primary key assigned by Dynamics.
            adoxioLegalEntity.Adoxio_legalentityid = dsr.GetAssignedId();

            return Json(adoxioLegalEntity.ToViewModel());
        }

        /// <summary>
        /// Update a legal entity
        /// </summary>
        /// <param name="item"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDynamicsLegalEntity([FromBody] ViewModels.AdoxioLegalEntity item, string id)
        {
            if (id != item.id)
            {
                return BadRequest();
            }
            DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Adoxio_legalentity> LegalEntityCollection = new DataServiceCollection<Interfaces.Microsoft.Dynamics.CRM.Adoxio_legalentity>(_system);

            // get the legal entity.
            Guid adoxio_legalentityid = new Guid(id);
            Adoxio_legalentity adoxioLegalEntity = await _system.Adoxio_legalentities.ByKey(adoxio_legalentityid).GetValueAsync();

            _system.UpdateObject(adoxioLegalEntity);
            // copy values over from the data provided
            adoxioLegalEntity.CopyValues(item, _system);

            // PostOnlySetProperties is used so that settings such as owner will get set properly by the dynamics server.

            DataServiceResponse dsr = _system.SaveChangesSynchronous(SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithIndependentOperations); // SaveChangesOptions.PostOnlySetProperties | SaveChangesOptions.BatchWithSingleChangeset
            foreach (OperationResponse result in dsr)
            {
                if (result.StatusCode == 500) // error
                {
                    return StatusCode(500, result.Error.Message);
                }
            }
            return Json(adoxioLegalEntity.ToViewModel());
        }

        /// <summary>
        /// Delete a legal entity.  Using a HTTP Post to avoid Siteminder issues with DELETE
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id}/delete")]
        public async Task<IActionResult> DeleteDynamicsLegalEntity(string id)
        {
            // get the legal entity.
            Guid adoxio_legalentityid = new Guid(id);
            try
            {
                Adoxio_legalentity adoxioLegalEntity = await _system.Adoxio_legalentities.ByKey(adoxio_legalentityid).GetValueAsync();
                _system.DeleteObject(adoxioLegalEntity);
				DataServiceResponse dsr = _system.SaveChangesSynchronous();
                foreach (OperationResponse result in dsr)
                {
                    if (result.StatusCode == 500) // error
                    {
                        return StatusCode(500, result.Error.Message);
                    }
                }
            }
            catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
            {
                return new NotFoundResult();
            }

            return NoContent(); // 204
        }
        /// <summary>
        /// Generate a link to be sent to an email address.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="individualId"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private string GetConsentLink(string email, string individualId, string parentId)
        {
            string result = Configuration["BASE_URI"] + Configuration["BASE_PATH"];

            result += "/security-consent/" + parentId + "/" + individualId + "?code=";

            // create a newsletter confirmation object.

            ViewModels.SecurityConsentConfirmation securityConsentConfirmation = new ViewModels.SecurityConsentConfirmation()
            {
                email = email,
                parentid = parentId,
                individualid = individualId
            };

            // convert it to a json string.
            string json = JsonConvert.SerializeObject(securityConsentConfirmation);

            // encrypt that using two way encryption.

            result += System.Net.WebUtility.UrlEncode(EncryptionUtility.EncryptString(json, _encryptionKey));

            return result;
        }

        [HttpGet("{id}/verifyconsentcode/{individualid}")]
        public JsonResult VerifyConsentCode(string id, string individualid, string code)
        {
            string result = "Error";
            // validate the code.

            string decrypted = EncryptionUtility.DecryptString(code, _encryptionKey);
            if (decrypted != null)
            {
                // convert the json back to an object.
                ViewModels.SecurityConsentConfirmation consentConfirmation = JsonConvert.DeserializeObject<ViewModels.SecurityConsentConfirmation>(decrypted);
                // check that the keys match.
                if (id.Equals(consentConfirmation.parentid) && individualid.Equals(consentConfirmation.individualid))
                {
                    // update the appropriate dynamics record here.
                    result = "Success";
                }
            }
            return Json(result);
        }


        /// <summary>
        /// send consent requests to the supplied list of legal entities.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="idList"></param>
        /// <returns></returns>
        [HttpPost("{id}/sendconsentrequests")]
        public async Task<IActionResult> SendConsentRequests(string id, [FromBody] List<string> recipientIds)
        {
            // start by getting the record for the current legal entity.

            // get the legal entity.
            Guid adoxio_legalentityid = new Guid(id);
            try
            {
                Adoxio_legalentity adoxioLegalEntity = await _system.Adoxio_legalentities.ByKey(adoxio_legalentityid).GetValueAsync();

                // now get each of the supplied ids and send an email to them.

                foreach (string recipientId in recipientIds)
                {
                    Guid recipientIdGuid = new Guid(recipientId);
                    try
                    {
                        Adoxio_legalentity recipientEntity = await _system.Adoxio_legalentities.ByKey(recipientIdGuid).GetValueAsync();
                        string email = recipientEntity.Adoxio_email;
                        string firstname = recipientEntity.Adoxio_firstname;
                        string lastname = recipientEntity.Adoxio_lastname;

                        string confirmationEmailLink = GetConsentLink(email, recipientId, id);
                        string bclogo = Configuration["BASE_URI"] + Configuration["BASE_PATH"] + "/assets/bc-logo.svg";
                        /* send the user an email confirmation. */
                        string body = "<img src='" + bclogo + "'/><br><h2>Security Check Consent</h2>"
                                     + "<p>Please confirm your security consent by clicking this link:</p>"
                                     + "<a href='" + confirmationEmailLink + "'>" + confirmationEmailLink + "</a>";

                        // send the email.
                        SmtpClient client = new SmtpClient(Configuration["SMTP_HOST"]);

                        // Specify the message content.
                        MailMessage message = new MailMessage("no-reply@gov.bc.ca", email);
                        message.Subject = "BC LCLB Cannabis Licensing Security Consent";
                        message.Body = body;
                        message.IsBodyHtml = true;
                        //client.Send(message);
                    }
                    catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
                    {
                        // ignore any not found errors.
                    }

                }

            }
            catch (Microsoft.OData.Client.DataServiceQueryException dsqe)
            {
                return new NotFoundResult();
            }

            return NoContent(); // 204
        }
    }
}
