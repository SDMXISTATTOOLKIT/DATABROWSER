using DataBrowser.Domain.Dtos;
using DataBrowser.Domain.Entities.DBoard;
using DataBrowser.Domain.Entities.SeedWork;
using DataBrowser.Domain.Entities.TransatableItems;
using DataBrowser.Domain.Entities.ViewTemplates.Validators;
using DataBrowser.Domain.Specifications.Rules;
using DataBrowser.Domain.Validators;
using DataBrowser.Interfaces.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataBrowser.Domain.Entities.DBoard
{
    public class Dashboard : Entity, IAggregateRoot
    {
        public int DashboardId { get; protected set; }
        public string DashboardConfig { get; set; }
        public int UserFk { get; protected set; }
        public int? HubFk { get; protected set; }
        public int TitleFK { get; protected set; }
        public int Weight { get; set; }
        public string FilterLevels { get; set; }
        

        private readonly List<DashboardViewTemplate> _views = new List<DashboardViewTemplate>();
        private readonly List<DashboardNode> _nodes = new List<DashboardNode>();

        //Navigation Properties
        public virtual TransatableItem Title { get; protected set; }
        public virtual IReadOnlyCollection<DashboardViewTemplate> Views => _views?.AsReadOnly();
        public virtual IReadOnlyCollection<DashboardNode> Nodes => _nodes?.AsReadOnly();

        public static async Task<IValidator<DashboardDto, Dashboard>> CreateDashboardAsync(DashboardDto dto, IEnumerable<IRuleSpecification<DashboardDto>> rules)
        {
            var validator = new Validator<DashboardDto, Dashboard>(rules);
            await validator.ExecuteCheckAsync(dto, new Dashboard());

            if (!validator.IsValid)
            {
                return validator;
            }


            validator.ValidateObject.DashboardConfig = dto.ConvertoDashboardConfigToText();
            validator.ValidateObject.UserFk = dto.UserId;
            validator.ValidateObject.Weight = dto.Weight;
            validator.ValidateObject.FilterLevels = dto.FilterLevels;

            if (dto.Title != null && dto.Title.Count > 0)
            {
                validator.ValidateObject.Title = TransatableItem.CreateTransatableItem(dto.Title);
            }

            if (dto.NodeIds != null)
            {
                validator.ValidateObject.SetNode(dto.NodeIds);
            }
            if (dto.ViewIds != null)
            {
                validator.ValidateObject.SetView(dto.ViewIds);
            }

            if (dto.HubId.HasValue)
            {
                validator.ValidateObject.SetHubAssociation(dto.HubId.Value);
            }

            return validator;
        }

        public async Task<IValidator<DashboardDto, Dashboard>> EditAsync(DashboardDto dto, IEnumerable<IRuleSpecification<DashboardDto>> rules)
        {
            var validator = new Validator<DashboardDto, Dashboard>(rules);
            await validator.ExecuteCheckAsync(dto, this);

            if (!validator.IsValid)
            {
                return validator;
            }

            SetTitleTranslation(dto.Title);
            DashboardConfig = dto.ConvertoDashboardConfigToText();
            FilterLevels = dto.FilterLevels;

            return validator;
        }

        public bool SetTitleTranslation(Dictionary<string, string> translates)
        {
            if (translates == null || translates.Count <= 0)
            {
                Title = null;
                return true;
            }

            if (Title == null)
            {
                Title = TransatableItem.CreateTransatableItem(translates);
                return true;
            }
            else if (Title.TransatableItemValues != null)
            { //Remove all old transalte
                Title.ClearTransatableItemValue();
            }
            //Reinsert all
            Title.AddTransatableItemValue(translates);

            return true;
        }

        public void RemoveHubAssociation()
        {
            this.HubFk = null;
        }

        public void SetHubAssociation(int hubFk)
        {
            this.HubFk = hubFk;
        }

        public void AssignNode(int nodeId)
        {
            var node = Nodes.FirstOrDefault(i => i.NodeId == nodeId);
            if (node != null)
            {
                return;
            }

            _nodes.Add(new DashboardNode { NodeId = nodeId });
        }

        public void UnAssignNode(int nodeId)
        {
            var node = Nodes.FirstOrDefault(i => i.NodeId == nodeId);
            if (node == null)
            {
                return;
            }

            _nodes.Remove(node);
        }

        public void SetNode(List<int> nodesId)
        {
            if (Nodes != null && Nodes.Count > 0)
            {
                _nodes.Clear();
            }

            foreach (var nodeId in nodesId)
            {
                if (_nodes.Any(i => i.NodeId == nodeId))
                {
                    continue;
                }

                _nodes.Add(new DashboardNode { NodeId = nodeId });
            }
        }

        public void AssignView(int viewId)
        {
            var view = Views.FirstOrDefault(i => i.ViewTemplateId == viewId);
            if (view != null)
            {
                return;
            }

            _views.Add(new DashboardViewTemplate { ViewTemplateId = viewId });
        }

        public void UnAssignView(int viewId)
        {
            var view = Views.FirstOrDefault(i => i.ViewTemplateId == viewId);
            if (view == null)
            {
                return;
            }

            _views.Remove(view);
        }

        public void SetView(List<int> viewsId)
        {
            if (Views != null && Views.Count > 0)
            {
                _views.Clear();
            }

            if (viewsId == null)
            {
                return;
            }

            foreach (var viewId in viewsId)
            {
                if (_views.Any(i => i.ViewTemplateId == viewId))
                {
                    continue;
                }

                _views.Add(new DashboardViewTemplate { ViewTemplateId = viewId });
            }
        }
    }
}
