﻿using OpenQA.Selenium;
using Protractor;
using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;
using System.IO;
using Serilog;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I click on the link for (.*)")]
        public void ClickOnLink(string specificLink)
        {
            NgWebElement uiRequestedLink = null;
            for (int i = 0; i < 30; i++)
            {
                try
                {
                    var names = ngDriver.FindElements(By.LinkText(specificLink));
                    if (names.Count > 0)
                    {
                        uiRequestedLink = names[0];
                        break;
                    }
                    else
                    {
                        System.Threading.Thread.Sleep(2000);
                    }
                }
                catch (Exception)
                {
                }
            }
            JavaScriptClick(uiRequestedLink);
        }


        [And(@"I click on the signature checkbox")]
        public void ClickOnSignatureCheckbox()
        {
            NgWebElement uiSignature = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='agreement']"));
            JavaScriptClick(uiSignature);
        }


        [And(@"I click on the branding change link for (.*)")]
        public void ClickOnBrandingChangeLink(string changeType)
        {
            /* 
            Page Title: Licences & Authorizations
            */

            string nameBrandingLinkCannabis = "Request Store Name or Branding Change";
            string nameBrandingLinkCateringMfg = "Establishment Name Change Application";

            if ((changeType == "Catering") || (changeType == "Manufacturing"))
            {
                // click on the Establishment Name Change Application link
                NgWebElement uiRequestChange = ngDriver.FindElement(By.LinkText(nameBrandingLinkCateringMfg));
                uiRequestChange.Click();
            }

            if (changeType == "Cannabis")
            {
                // click on the Request Store Name or Branding Change link
                NgWebElement uiRequestChange = ngDriver.FindElement(By.LinkText(nameBrandingLinkCannabis));
                uiRequestChange.Click();
            }
        }


        [And(@"I click on the Licences tab")]
        public void ClickLicencesTab()
        {
            NgWebElement uiRequestChange = ngDriver.FindElement(By.LinkText("Licences & Authorizations"));
            JavaScriptClick(uiRequestChange);
        }


        [And(@"I click on the Dashboard tab")]
        public void ClickDashboardTab()
        {
            try
            {
                NgWebElement uiRequestChange = ngDriver.FindElement(By.LinkText("Dashboard"));
                JavaScriptClick(uiRequestChange);
            }
            catch
            {
                Log.Logger.Information("Unable to find the Dashboard tab. Navigating there directly.");
                ngDriver.Navigate().GoToUrl($"{baseUri}dashboard");
            }
        }


        [And(@"I click on the button for (.+)")]
        public void ClickOnButton(string specificButton)
        {
            if (specificButton == "CRS terms and conditions")
            {
                // click on the Terms and Conditions button
                NgWebElement uiTermsAndConditions = ngDriver.FindElement(By.CssSelector("mat-expansion-panel mat-expansion-panel-header[role='button']"));
                uiTermsAndConditions.Click();
            }

            if (specificButton == "Catering terms and conditions")
            {
                // click on the Terms and Conditions button
                NgWebElement uiTermsAndConditions = ngDriver.FindElement(By.CssSelector("mat-expansion-panel mat-expansion-panel-header[role='button']"));
                uiTermsAndConditions.Click();
            }

            if (specificButton == "Confirm Organization Information is Complete")
            {
                // click on the Confirm Organization Information is Complete button
                NgWebElement uiCompleteButton = ngDriver.FindElement(By.CssSelector("app-application-licensee-changes button.btn-primary"));
                uiCompleteButton.Click();
            }

            if (specificButton == "Pay for Application")
            {
                // click on the Pay for Application button
                NgWebElement uiPayButton = ngDriver.FindElement(By.CssSelector("button.btn-primary"));
                uiPayButton.Click();
            }

            if (specificButton == "Pay for Application for Cannabis Marketing")
            {
                // click on the Pay for Application button
                NgWebElement uiPayButton = ngDriver.FindElement(By.CssSelector(".mt-3 button.btn.btn-primary"));
                uiPayButton.Click();
            }

            if (specificButton == "Submit Organization Information")
            {
                // click on the Submit Org Info button
                NgWebElement uiSubmitOrgInfoButton = ngDriver.FindElement(By.CssSelector("app-application-licensee-changes button.btn-primary"));
                JavaScriptClick(uiSubmitOrgInfoButton);
            }

            if (specificButton == "Save for Later")
            {
                // click on the Save For Later button
                NgWebElement uiSaveForLaterButton = ngDriver.FindElement(By.CssSelector("button.btn-secondary span"));
                uiSaveForLaterButton.Click();
            }

            if (specificButton == "Continue to Organization Review")
            {
                // click on the Continue to Organization Review button
                NgWebElement uiContinueToOrganizationReview = ngDriver.FindElement(By.CssSelector("button#continueToApp"));
                uiContinueToOrganizationReview.Click();
            }

            if (specificButton == "Proceed to Security Screening")
            {
                // click on the Proceed to Security Screening button
                NgWebElement uiProceedToSecurityScreening = ngDriver.FindElement(By.CssSelector("button.mat-primary"));
                uiProceedToSecurityScreening.Click();
            }

            if (specificButton == "Submit a Change")
            {
                // click on the Submit a Change button
                NgWebElement uiSubmitAChange = ngDriver.FindElement(By.CssSelector("button.mat-primary"));
                IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
                executor.ExecuteScript("arguments[0].scrollIntoView(true);", uiSubmitAChange);
                uiSubmitAChange.Click();
            }
        }


        [And(@"I click on the Submit button")]
        public void ClickOnSubmitButton()
        {
            NgWebElement uiSubmitButton = ngDriver.FindElement(By.CssSelector("button.mat-primary"));
            JavaScriptClick(uiSubmitButton);
        }


        [And(@"I click on the secondary Submit button")]
        public void ClickOnSubmitButton2()
        {
            NgWebElement uiSubmitButton = ngDriver.FindElement(By.CssSelector("button.btn-primary"));
            JavaScriptClick(uiSubmitButton);
        }


        [And(@"I click on the LG Submit button")]
        public void ClickOnLGSubmitButton()
        {
            NgWebElement uiSubmitButton = ngDriver.FindElement(By.CssSelector("button.mat-raised-button.ng-test-submit-for-lg-review"));
            JavaScriptClick(uiSubmitButton);
        }


        [And(@"I click on the overlay Submit button")]
        public void ClickOnOverlaySubmitButton()
        {
            NgWebElement uiSubmitButton = ngDriver.FindElement(By.CssSelector(".cdk-global-overlay-wrapper button.mat-primary"));
            JavaScriptClick(uiSubmitButton);
        }


        [And(@"I click on the Continue to Application button")]
        public void ContinueToApplicationButton()
        {
            // click on the Continue to Application button
            NgWebElement uiContinueButton = ngDriver.FindElement(By.CssSelector("button#continueToApp"));
            JavaScriptClick(uiContinueButton);
        }


        [And(@"I click on the Review Organization Information button")]
        public void ClickReviewOrganizationInformation()
        {
            // click on the review organization information button
            NgWebElement uiOrgInfoButton = ngDriver.FindElement(By.CssSelector("button.btn-primary[routerlink='/org-structure']"));
            uiOrgInfoButton.Click();
        }


        [And(@"I click on the Complete Organization Information button")]
        public void CompleteOrgInfo()
        {
            // click on the complete organization information button
            NgWebElement uiOrgInfoButton = ngDriver.FindElement(By.CssSelector("button.btn-primary[routerlink='/org-structure']"));
            uiOrgInfoButton.Click();
        }


        [And(@"I click on the Start Application button for (.+)")]
        public void ClickStartApplication(string applicationType)
        {
            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */
            Log.Logger.Information("ENTERING ClickStartApplication");
            ngDriver.WaitForAngular();

            switch (applicationType)
            {
                case "Catering":
                    // click on the Catering Start Application button
                    var startCatering = ngDriver.WrappedDriver.FindElement(By.CssSelector("button[id='startCatering']"));
                    JavaScriptClick(startCatering);
                    break;
                case "a Cannabis Retail Store":
                    // click on the Cannabis Start Application button
                    var startCRS = ngDriver.WrappedDriver.FindElement(By.CssSelector("button[id='startCRS']"));
                    JavaScriptClick(startCRS);
                    break;
                case "a Rural Agency Store":
                    // click on the Rural Store Start Application button
                    var startRas = ngDriver.WrappedDriver.FindElement(By.CssSelector("button[id='startRAS']"));
                    JavaScriptClick(startRas);
                    break;
                case "a Manufacturer Licence":
                    var startMfg = ngDriver.WrappedDriver.FindElement(By.CssSelector("button[id='startMfg']"));
                    JavaScriptClick(startMfg);
                    break;
                case "a Cannabis Marketing Licence":
                    // click on the Cannabis Marketing Licence Start Application button
                    var startMarketing = ngDriver.FindElement(By.CssSelector("button[id='startMarketing']"));
                    JavaScriptClick(startMarketing);
                    break;
                case "a UBrew UVin application":
                    // click on the UBrew UVin application Licence Start Application button
                    var startUbv = ngDriver.FindElement(By.CssSelector("button[id='startUBV']"));
                    JavaScriptClick(startUbv);
                    break;
                case "Food Primary":
                    // click on the Food Primary Start Application button
                    var startFp = ngDriver.FindElement(By.CssSelector("button[id='startFP']"));
                    JavaScriptClick(startFp);
                    break;
                case "a Liquor Primary Licence":
                    // click on the Liquor Primary Start Application button
                    var startLP = ngDriver.FindElement(By.CssSelector("button[id='startLP']"));
                    JavaScriptClick(startLP);
                    break;
                case "a LPC Licence":
                    // click on the Liquor Primary Club Start Application button
                    var startLPC = ngDriver.FindElement(By.CssSelector("button[id='startLPC']"));
                    JavaScriptClick(startLPC);
                    break;
                case "Rural LRS":
                    // click on the Rural LRS Start Application button
                    var startRLRS = ngDriver.FindElement(By.CssSelector("button[id='startRLRS']"));
                    JavaScriptClick(startRLRS);
                    break;
                case "an Agent Licence":
                    // click on the Agent Licence button
                    var startAgent = ngDriver.FindElement(By.CssSelector("button[id='startAgent']"));
                    JavaScriptClick(startAgent);
                    break;
            }
        }

        public void SharedCalendarDate()
        {
            // click on the previous button
            NgWebElement uiOpenCalendarPrevious = ngDriver.FindElement(By.CssSelector(".mat-calendar .mat-calendar-previous-button"));
            JavaScriptClick(uiOpenCalendarPrevious);

            // click on the first day
            NgWebElement uiOpenCalendarYear = ngDriver.FindElement(By.CssSelector(".mat-calendar-content .mat-calendar-body-cell-content:first-child"));
            JavaScriptClick(uiOpenCalendarYear);
        }

        private string MakeAPICall(string url)
        {
            NgWebElement apiInput = ngDriver.FindElement(By.Id("testUrl"));
            apiInput.SendKeys($"{url}");

            NgWebElement inputButton = ngDriver.FindElement(By.Id("testAPIButton"));
            inputButton.Click();

            NgWebElement apiResult = ngDriver.FindElement(By.Id("testAPIResult"));
            var text = apiResult.Text;
            int maxTries = 15;
            int tries = 0;
            do
            {
                text = apiResult.Text;
                System.Threading.Thread.Sleep(2000);
                tries++;
            } while (tries < maxTries && string.IsNullOrEmpty(text));

            return text;
        }


        [And(@"the application is approved")]
        public void ApplicationIsApproved()
        {
            string result = MakeAPICall($"{baseUri}api/applications/{applicationID}/process");
            Assert.Contains("OK", result);

            ClickLicencesTab();
        }


        [And(@"the on-site endorsement application is approved")]
        public void OnSiteEndorsementApplicationIsApproved()
        {
            string result = MakeAPICall($"{baseUri}api/applications/{endorsementID}/processEndorsement");
            Assert.Contains("OK", result);

            ClickLicencesTab();
        }


        [And(@"I do not complete the application correctly")]
        public void CompleteApplicationIncorrectly()
        {
            System.Threading.Thread.Sleep(5000);

            ClickOnSubmitButton();
        }


        [And(@"autorenewal is set to 'No'")]
        public void AutoRenewalDenied()
        {
            string renewLicence = "Transfer Licence";

            // find the Transfer Licence link
            NgWebElement uiLicenceID = ngDriver.FindElement(By.LinkText(renewLicence));
            string URL = uiLicenceID.GetAttribute("href");

            // retrieve the licence ID
            string[] parsedURL = URL.Split('/');

            licenceID = parsedURL[5];

            ngDriver.IgnoreSynchronization = true;

            // navigate to api/Licenses/noautorenew/{licenceID}
            ngDriver.Navigate().GoToUrl($"{baseUri}api/Licenses/noautorenew/{licenceID}");

            if (!ngDriver.WrappedDriver.PageSource.Contains("OK"))
            {
                throw new Exception(ngDriver.WrappedDriver.PageSource);
            }

            ngDriver.IgnoreSynchronization = false;

            // navigate back to Licenses tab
            ngDriver.Navigate().GoToUrl($"{baseUri}licences");
        }


        [And(@"I am unable to renew the licence")]
        public void RenewalLinkHidden()
        {
            Assert.True(ngDriver.FindElement(By.XPath("//body[not(contains(.,'Renew Licence'))]")).Displayed);
        }


        [And(@"I do not complete the licence renewal application correctly")]
        public void CompleteApplicationRenewalIncorrectly()
        {
            // click on the Submit button
            NgWebElement uiSubmitButton = ngDriver.FindElement(By.CssSelector("button:nth-child(2)"));
            uiSubmitButton.Click();
        }


        [And(@"the expiry date is changed using the Dynamics workflow named (.*)")]
        public void SetExpiryDate(string workflowGUID)
        {
            string transferLicence = "Transfer Licence";

            // find the Transfer Licence link
            NgWebElement uiLicenceID = ngDriver.FindElement(By.LinkText(transferLicence));
            string URL = uiLicenceID.GetAttribute("href");

            // retrieve the licence ID
            string[] parsedURL = URL.Split('/');

            licenceID = parsedURL[5];

            ngDriver.IgnoreSynchronization = true;

            string result = MakeAPICall($"{baseUri}api/Licenses/{workflowGUID}/setexpiry/{licenceID}");
            Assert.Contains("OK", result);
            ClickDashboardTab(); // navigate away the back to cause data reload
            ClickLicencesTab();
        }


        [And(@"I show the store as open on the map")]
        public void ShowStoreOpenOnMap()
        {
            /* 
            Page Title: Licences & Authorizations
            Subtitle:   Cannabis Retail Store Licences
            */

            // click on the Show Store as Open on Map checkbox
            NgWebElement uiMapCheckbox = ngDriver.FindElement(By.CssSelector("mat-checkbox"));
            uiMapCheckbox.Click();
        }


        [And(@"the licence is successfully downloaded")]
        public void SuccessfulLicenceDownload()
        {
            /* 
            Page Title: Licences & Authorizations
            */

            // get the licence ID - use a different link than renewLicence
            // NgWebElement uiLicenceID = ngDriver.FindElement(By.LinkText(renewLicence));
            // string URL = uiLicenceID.GetAttribute("href");
            // string[] parsedURL = URL.Split('/');
            // licenceID = parsedURL[5];

            // navigate to the Downloads folder
            // var environment = Environment.CurrentDirectory;

            // check if the licence ID matches by name  
        }


        [And(@"the dashboard status is updated as (.*)")]
        public void DashboardStatus(string status)
        {
            if (status == "Application Under Review")
            {
                Assert.True(ngDriver.FindElement(By.XPath($"//body[contains(.,' Application Under Review ')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath($"//body[contains(.,'Add Supporting Documents')]")).Displayed);
            }

            if (status == "Pending External Review")
            {
                Assert.True(ngDriver.FindElement(By.XPath($"//body[contains(.,' Pending External Review ')]")).Displayed);
                Assert.True(ngDriver.FindElement(By.XPath($"//body[contains(.,'Complete Application')]")).Displayed);
            }
        }


        public void FileUpload(string fileName, string inputFile)
        {
            var elements = ngDriver.FindElements(By.XPath(inputFile));
            if (elements == null || elements.Count == 0)
            {
                // try again
                elements = ngDriver.FindElements(By.XPath(inputFile));
            }

            NgWebElement uiUploadDocument = elements[0];

            // find the upload test files in the bdd-tests\upload_files folder
            var environment = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(environment).Parent.FullName;
            string projectDirectory2 = Directory.GetParent(projectDirectory).Parent.FullName;

            // upload the document
            string documentPath = Path.Combine(projectDirectory2 + Path.DirectorySeparatorChar + "bdd-tests" + Path.DirectorySeparatorChar + "upload_files" + Path.DirectorySeparatorChar + fileName);
            uiUploadDocument.SendKeys(documentPath);

            // wait for upload to finish
            int maxTries = 10;
            int tries = 0;
            bool found = false;
            ngDriver.IgnoreSynchronization = true;
            var tempTimeout = ngDriver.Manage().Timeouts().ImplicitWait;
            ngDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(0.25);

            while (!found && tries < maxTries)
            {
                try
                {
                    // check to see if a file was uploaded
                    var path = $"{inputFile}/ancestor::app-file-uploader//*[contains(concat(' ', normalize-space(@class), ' '), ' file-list ')]//a";
                    NgWebElement uploadedFile = ngDriver.FindElement(By.XPath(inputFile));
                    found = true;
                }
                catch (Exception e)
                {
                    // do nothing
                }
                tries++;
            }
            ngDriver.IgnoreSynchronization = false;
            ngDriver.Manage().Timeouts().ImplicitWait = tempTimeout;
        }


        public void JavaScriptClick(IWebElement element)
        {
            IJavaScriptExecutor executor = (IJavaScriptExecutor)(ngDriver.WrappedDriver);
            executor.ExecuteScript("arguments[0].click();", element);
        }

        [And(@"No applications awaiting review is displayed")]
        public void NoApplicationsAwaitingReview()
        {
            //Confirm that "No applications awaiting review" message is displayed
            Assert.True(ngDriver.FindElement(By.XPath($"//body[contains(.,'No applications awaiting review')]")).Displayed);
        }
    }
}
