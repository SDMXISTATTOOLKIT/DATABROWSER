using System.Collections.Generic;
using System.Linq;
using DataBrowser.AC.Utility.Helpers.ExtMethod;
using DataBrowser.Command.Dashboards.Model;
using DataBrowser.Command.ViewTemplates.Model;
using DataBrowser.Interfaces.Dto.Users;

namespace WSHUB.Models.Response
{
    public class DeleteGenericEntityModelView
    {
        public bool DeleteResult { get; set; }
        public List<GenericEntity> UsedBy { get; set; }

        public static DeleteGenericEntityModelView ConvertFromViewTemplate(
            RemoveViewTemplateResult removeViewTemplateResult, string lang)
        {
            var result = new DeleteGenericEntityModelView
            {
                DeleteResult = removeViewTemplateResult.Deleted
            };
            result.UsedBy = removeViewTemplateResult.Dashboards?.Select(i => new GenericEntity
                {Type = "dashboard", Id = i.DashboardId, Title = i.Title.GetTranslateItem(lang)})?.ToList();

            return result;
        }

        public static DeleteGenericEntityModelView ConvertFromUser(UserDeleteDto userDeleteDto, string lang)
        {
            var result = new DeleteGenericEntityModelView
            {
                DeleteResult = userDeleteDto.Deleted
            };
            if (userDeleteDto.Dashboards != null)
            {
                result.UsedBy = new List<GenericEntity>();
                foreach (var dash in userDeleteDto.Dashboards)
                {
                    var genericEntity = new GenericEntity
                        {Type = "dashboard", Id = dash.Id, Title = dash.Title.GetTranslateItem(lang)};
                    var nodeUsed = dash.Nodes?.Select(i => new GenericEntity
                        {Type = "node", Id = i.Id, Title = i.Title.GetTranslateItem(lang)})?.ToList();
                    if (nodeUsed != null) genericEntity.AssociatedEntities = nodeUsed;
                    var viewUsed = dash.Views?.Select(i => new GenericEntity
                        {Type = "view", Id = i.Id, Title = i.Title.GetTranslateItem(lang)})?.ToList();
                    if (viewUsed != null)
                    {
                        if (genericEntity.AssociatedEntities == null)
                            genericEntity.AssociatedEntities = new List<GenericEntity>();
                        genericEntity.AssociatedEntities.AddRange(viewUsed);
                    }

                    result.UsedBy.Add(genericEntity);
                }
            }


            return result;
        }

        public static DeleteGenericEntityModelView ConvertFromDashboard(RemoveDashboardResult removeDashboardResult,
            string lang)
        {
            var result = new DeleteGenericEntityModelView
            {
                DeleteResult = removeDashboardResult.Deleted
            };
            result.UsedBy = removeDashboardResult.Nodes?.Select(i => new GenericEntity
                {Type = "node", Id = i.NodeId, Title = i.Title.GetTranslateItem(lang)})?.ToList();

            if (removeDashboardResult.AssignToHub)
            {
                if (result.UsedBy == null) result.UsedBy = new List<GenericEntity>();
                result.UsedBy.Add(new GenericEntity {Type = "hub", Id = 1, Title = "Hub"});
            }

            return result;
        }

        public class GenericEntity
        {
            public string Type { get; set; }
            public int Id { get; set; }
            public string Title { get; set; }
            public List<GenericEntity> AssociatedEntities { get; set; }
        }
    }
}