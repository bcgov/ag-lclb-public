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
Feature: CRSRequestAStructuralChange
    As a logged in business user
    I want to submit a CRS Application for different business types
    And request a structural change for the approved application

@cannabis @privatecorporation @crsstructuralchanges
Scenario: Cannabis Structural Changes (Private Corporation)
    Given I am logged in to the dashboard as a private corporation
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a private corporation
    And I complete the Cannabis Retail Store application for a private corporation
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I request a structural change
    And I click on the link for Dashboard
    And I confirm the structural change request is displayed on the dashboard
    And the account is deleted
    Then I see the login page

@cannabis @society @crsstructuralchanges
Scenario: Cannabis Structural Changes (Society)
    Given I am logged in to the dashboard as a society
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a society
    And I complete the Cannabis Retail Store application for a society
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I request a structural change
    And I click on the link for Dashboard
    And I confirm the structural change request is displayed on the dashboard
    And the account is deleted
    Then I see the login page

@cannabis @partnership @crsstructuralchanges
Scenario: Cannabis Structural Changes (Partnership)
    Given I am logged in to the dashboard as a partnership
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a partnership
    And I complete the Cannabis Retail Store application for a partnership
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I request a structural change
    And I click on the link for Dashboard
    And I confirm the structural change request is displayed on the dashboard
    And the account is deleted
    Then I see the login page

@cannabis @soleproprietorship @crsstructuralchanges
Scenario: Cannabis Structural Changes (Sole Proprietorship)
    Given I am logged in to the dashboard as a sole proprietorship
    And I click on the Start Application button for a Cannabis Retail Store
    # And I complete the eligibility disclosure
    And I review the account profile for a sole proprietorship
    And I complete the Cannabis Retail Store application for a sole proprietorship
    And I click on the Submit button
    And I enter the payment information
    And the application is approved
    And I pay the licensing fee
    And I request a structural change
    And I click on the link for Dashboard
    And I confirm the structural change request is displayed on the dashboard
    And the account is deleted
    Then I see the login page
*/

namespace bdd_tests
{
    [FeatureFile("./CRSRequestAStructuralChange.feature")]
    [Collection("Cannabis")]
    public sealed class CRSRequestAStructuralChange : TestBase
    {
        [Given(@"I am logged in to the dashboard as a(.*)")]
        public void LogInToDashboard(string businessType)
        {
            NavigateToFeatures();

            CheckFeatureFlagsLGIN();

            CheckFeatureFlagsIN();

            CheckFeatureFlagsLicenseeChanges();

            CheckFeatureFlagsSecurityScreening();

            CheckFeatureLEConnections();

            IgnoreSynchronizationFalse();

            CarlaLogin(businessType);
        }
    }
}
