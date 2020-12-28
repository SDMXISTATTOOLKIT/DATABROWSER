using System.Collections.Generic;

namespace WSHUB.Models.Response
{
    public class EditUsersPermissionsModelView
    {
        public bool HaveError { get; set; }
        public List<int> ErrorsUserId { get; set; }
    }
}