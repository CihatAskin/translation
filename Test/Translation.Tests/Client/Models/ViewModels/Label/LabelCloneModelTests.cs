﻿using System;
using System.Collections;

using NUnit.Framework;
using Shouldly;

using Translation.Client.Web.Models.Label;
using Translation.Common.Helpers;
using static Translation.Tests.TestHelpers.FakeModelTestHelper;
using static Translation.Tests.TestHelpers.FakeConstantTestHelper;
using static Translation.Tests.TestHelpers.AssertViewModelTestHelper;

namespace Translation.Tests.Client.Models.ViewModels.Label
{
    [TestFixture]
    public class LabelCloneModelTests
    {
        public LabelCloneModel SystemUnderTest { get; set; }

        [SetUp]
        public void run_before_every_test()
        {
            SystemUnderTest = GetLabelCloneModel();
        }

        [Test]
        public void LabelCloneModel_Title()
        {
            Assert.AreEqual(SystemUnderTest.Title, "label_clone_title");
        }

        [Test]
        public void LabelCloneModel_OrganizationUidInput()
        {
            AssertHiddenInputModel(SystemUnderTest.OrganizationUidInput, "OrganizationUid");
        }

        [Test]
        public void LabelCloneModel_CloningLabelUidInput()
        {
            AssertHiddenInputModel(SystemUnderTest.CloningLabelUidInput, "CloningLabelUid");
        }

        [Test]
        public void LabelCloneModel_CloningLabelKeyInput()
        {
            AssertHiddenInputModel(SystemUnderTest.CloningLabelKeyInput, "CloningLabelKey");
        }

        [Test]
        public void LabelCloneModel_ProjectUidInput()
        {
            AssertSelectInputModel(SystemUnderTest.ProjectUidInput, "ProjectUid", "ProjectName", "project", "/Project/SelectData/");
        }

        [Test]
        public void LabelCloneModel_KeyInput()
        {
            AssertInputModel(SystemUnderTest.KeyInput, "Key", "key");
        }

        [Test]
        public void LabelCloneModel_DescriptionInput()
        {
            AssertInputModel(SystemUnderTest.DescriptionInput, "Description", "description");
        }

        [Test]
        public void LabelCloneModel_CloningLabelTranslationCountInput()
        {
            AssertHiddenInputModel(SystemUnderTest.CloningLabelTranslationCountInput, "CloningLabelTranslationCount", "cloning_label_translation_count");
        }

        [Test]
        public void LabelCreateModel_SetInputModelValues()
        {
            // arrange

            // act
            SystemUnderTest.SetInputModelValues();

            // assert
            SystemUnderTest.OrganizationUidInput.Value.ShouldBe(SystemUnderTest.OrganizationUid.ToUidString());
            SystemUnderTest.CloningLabelUidInput.Value.ShouldBe(SystemUnderTest.CloningLabelUid.ToUidString());
            SystemUnderTest.CloningLabelKeyInput.Value.ShouldBe(SystemUnderTest.CloningLabelKey);
            SystemUnderTest.ProjectUidInput.Value.ShouldBe(SystemUnderTest.ProjectUid.ToUidString());
            SystemUnderTest.KeyInput.Value.ShouldBe(SystemUnderTest.CloningLabelKey);
            SystemUnderTest.DescriptionInput.Value.ShouldBe(SystemUnderTest.CloningLabelDescription);
            SystemUnderTest.CloningLabelTranslationCountInput.Value.ShouldBe(SystemUnderTest.CloningLabelTranslationCount.ToString());
        }

        public static IEnumerable MessageTestCases
        {
            get
            {
                yield return new TestCaseData(CaseOne,
                                              UidOne, UidTwo, StringOne,
                                              StringTwo, UidThree, StringThree,
                                              null,
                                              null,
                                              true);

                yield return new TestCaseData(CaseTwo,
                                              EmptyUid, EmptyUid, EmptyString,
                                              EmptyString, EmptyUid, EmptyString,
                                              new[] { "organization_uid_not_valid",
                                                      "cloning_label_uid_not_valid",
                                                      "cloning_label_key_required" },
                                              new[] { "project_required_error_message",
                                                      "key_required_error_message" },
                                              false);
            }
        }

        [TestCaseSource(nameof(MessageTestCases))]
        public void LabelCloneModel_InputErrorMessages(string caseName,
                                                       Guid organizationUid, Guid cloningLabelUid, string cloningLabelKey,
                                                       string cloningLabelDescription, Guid projectUid, string key,
                                                       string[] errorMessages,
                                                       string[] inputErrorMessages,
                                                       bool result)
        {
            var model = GetLabelCloneModel(organizationUid, cloningLabelUid, cloningLabelKey,
                                           cloningLabelDescription, projectUid, key);
            model.IsValid().ShouldBe(result);
            model.IsNotValid().ShouldBe(!result);

            AssertMessages(model.ErrorMessages, errorMessages);
            AssertMessages(model.InputErrorMessages, inputErrorMessages);
        }
    }
}
