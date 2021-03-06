﻿using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using Protractor;
using System;
using Xunit;
using Xunit.Abstractions;
using Xunit.Gherkin.Quick;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace bdd_tests
{
    public abstract partial class TestBase : Feature, IDisposable
    {
        [And(@"I request a third party operator")]
        public void RequestThirdPartyOperator()
        {
            // navigate back to Licenses tab
            ClickLicencesTab();

            /* 
            Page Title: Licences & Authorizations
            Subtitle:   Catering Licences
            */

            string addOrChangeThirdParty = "Add or Change a Third Party Operator";

            // click on the Add or Change a Third Party Operator Link
            NgWebElement uiAddOrChangeThirdPartyOp = ngDriver.FindElement(By.LinkText(addOrChangeThirdParty));
            uiAddOrChangeThirdPartyOp.Click();

            /* 
            Page Title: Add or Change a Third Party Operator
            */
            
            // check that licence number field is populated
            NgWebElement uiLicenseNumber = ngDriver.FindElement(By.CssSelector("input[formcontrolname='licenseNumber']"));
            string fieldValueLicenseNumber = uiLicenseNumber.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueLicenseNumber));

            // check that establishment name field is populated
            NgWebElement uiEstablishmentName = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentName']"));
            string fieldValueEstablishmentName = uiEstablishmentName.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentName));

            // check that establishment address street field is populated
            NgWebElement uiEstablishmentAddressStreet = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressStreet']"));
            string fieldValueEstablishmentAddressStreet = uiEstablishmentAddressStreet.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentAddressStreet));

            // check that establishment address city field is populated
            NgWebElement uiEstablishmentAddressCity = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressCity']"));
            string fieldValueEstablishmentAddressCity = uiEstablishmentAddressCity.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentAddressCity));

            // check that establishment province field is populated
            NgWebElement uiEstablishmentProvince = ngDriver.FindElement(By.CssSelector("input.form-control[value='British Columbia']"));
            string fieldValueEstablishmentProvince = uiEstablishmentProvince.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentProvince));

            // check that establishment postal code field is populated
            NgWebElement uiEstablishmentAddressPostalCode = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentAddressPostalCode']"));
            string fieldValueEstablishmentAddressPostalCode = uiEstablishmentAddressPostalCode.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentAddressPostalCode));

            // check that establishment country field is populated
            NgWebElement uiEstablishmentAddressCountry = ngDriver.FindElement(By.CssSelector("input.form-control[value='Canada']"));
            string fieldValueEstablishmentCountry = uiEstablishmentAddressCountry.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentCountry));

            // check that establishment PID is populated
            NgWebElement uiEstablishmentParcelId = ngDriver.FindElement(By.CssSelector("input[formcontrolname='establishmentParcelId']"));
            string fieldValueEstablishmentParcelId = uiEstablishmentParcelId.GetProperty("value");
            Assert.False(string.IsNullOrEmpty(fieldValueEstablishmentParcelId));

            // create test data
            string thirdparty = "GunderCorp";

            // search for the proposed licensee
            NgWebElement uiThirdPartyOperator = ngDriver.FindElement(By.CssSelector("input[formcontrolname='autocompleteInput']"));
            uiThirdPartyOperator.SendKeys(thirdparty);

            NgWebElement uiThirdPartyOperatorOption = ngDriver.FindElement(By.CssSelector("mat-option[role='option'] span"));
            JavaScriptClick(uiThirdPartyOperatorOption);

            // click on authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // click on signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();

            // click on submit button
            ClickOnSubmitButton2();

            // navigate back to Licenses tab
            ClickLicencesTab();

            /* 
            Page Title: Welcome to Liquor and Cannabis Licensing
            */

            ngDriver.Navigate().Refresh();

            // confirm that the application has been initiated
            Assert.True(ngDriver.FindElement(By.XPath("//body[contains(.,'Third Party Operator Application Initiated')]")).Displayed);
        }


        [And(@"I cancel the third party operator application")]
        public void CancelThirdPartyOperator()
        {
            /* 
            Page Title: Cancel Third Party Operator Application
            */

            // click on agreement checkbox
            NgWebElement uiTransferConsent = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='transferConsent']"));
            uiTransferConsent.Click();

            // click on Cancel Third Party Application button
            ClickOnSubmitButton2();
        }
    }
}
