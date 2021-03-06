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
        [And(@"I request a Patron Participation Entertainment Endorsement application")]
        public void PatronParticipationApplication()
        {
            /* 
            Page Title: Please Review Your Account Profile
            */

            // click on Continue to Application button
            ContinueToApplicationButton();

            /* 
            Page Title: Patron Participation Entertainment Endorsement Application
            */

            // create test data
            string patronEntertainment = "Sample patron entertainment.";

            // enter the patron entertainment
            NgWebElement uiPatronEntertainment = ngDriver.FindElement(By.CssSelector("textarea#description2"));
            uiPatronEntertainment.SendKeys(patronEntertainment);

            // click on authorized to submit checkbox
            NgWebElement uiAuthorizedToSubmit = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='authorizedToSubmit']"));
            uiAuthorizedToSubmit.Click();

            // click on signature agreement checkbox
            NgWebElement uiSignatureAgreement = ngDriver.FindElement(By.CssSelector("mat-checkbox[formcontrolname='signatureAgreement']"));
            uiSignatureAgreement.Click();
        }
    }
}
