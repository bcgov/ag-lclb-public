﻿using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support;
using OpenQA.Selenium.Support.UI;
using Protractor;
using System;
using Xunit.Gherkin.Quick;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.IO;
using Xunit;

/*
Feature: LiquorPrimaryTempUseAreaEndorsementApproved
    As a logged in business user
    I want to request a temporary use area endorsement for a Liquor Primary Application

@liquorprimary
Scenario: Liquor Primary Temp Use Area Endorsement Approved (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Liquor Primary Licence
    And I review the account profile for a private corporation
    And I complete the Liquor Primary application for a private corporation
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority for liquor primary
    And I click on the Submit button
    And I click on the overlay Submit button
    And No applications awaiting review is displayed
    And I log in as a return user
    And I click on the link for Complete Application
    And I review the local government response for a liquor primary licence
    And the application is approved
    And I click on the Licences tab
    And I pay the licensing fee
    And I click on the link for Temporary Use Area Endorsement Application
    And I submit a temporary use area endorsement application
    And I click on the Submit button
    And I log in as local government for Parksville
    And I click on the link for Applications for Review
    And I click on the link for Review Application
    And I specify my contact details as the approving authority for TUA
    And I click on the Submit button
    And I click on the overlay Submit button	
    And No applications awaiting review is displayed
    And I log in as a return user
    And I click on the link for Complete Application
    And I click on the Continue to Application button	
    And I click on the Submit button	
    And I enter the payment information
    And the on-site endorsement application is approved
    And I click on the Licences tab
    And I click on the link for Request Temporary Use Area Event Authorization
    And I complete the TUA event application 
    And I click on the Submit button
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./LiquorPrimaryTempUseAreaEndorsementApproved.feature")]
    [Collection("Liquor")]
    public sealed class LiquorPrimaryTempUseAreaEndorsementApproved : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLiquorTwo();

            CheckFeatureFlagsLicenseeChanges();

            CheckFeatureLEConnections();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
