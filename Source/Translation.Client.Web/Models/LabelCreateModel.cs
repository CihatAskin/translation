﻿using Translation.Client.Web.Helpers;
using Translation.Client.Web.Models.InputModels;
using Translation.Common.Helpers;

namespace Translation.Client.Web.Models
{
    public class LabelCreateModel : BaseModel
    {
        public string Project { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public SelectInputModel ProjectInput { get; set; }
        public InputModel NameInput { get; set; }
        public LongInputModel DescriptionInput { get; set; }

        public LabelCreateModel()
        {
            Title = Localizer.Localize("label_create_title");

            ProjectInput = new SelectInputModel("Project", "project", true);
            NameInput = new InputModel("Name", "name", true);
            DescriptionInput = new LongInputModel("Description", "description");
        }

        public override void SetInputModelValues()
        {
            ProjectInput.Value = Project;
            NameInput.Value = Name;
            DescriptionInput.Value = Description;
        }

        public override void SetInputErrorMessages()
        {
            if (Name.IsEmpty())
            {
                NameInput.ErrorMessage.Add("name_required_error_message");
                ErrorMessages.AddRange(NameInput.ErrorMessage);
            }

            if (Project.IsEmpty())
            {
                ProjectInput.ErrorMessage.Add("project_required_error_message");
                ErrorMessages.AddRange(ProjectInput.ErrorMessage);
            }
        }
    }
}