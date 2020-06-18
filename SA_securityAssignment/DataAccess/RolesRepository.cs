using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace DataAccess
{
    public class RolesRepository : ConnectionClass
    {
        public RolesRepository() : base() {
        }

       
        public Role GetRole(string role) {
            return Entity.Roles.SingleOrDefault(x => x.Role1 == role);
        }
        
        //Currently not using this overload.
        public Role GetRole(int roleId)
        {
            return Entity.Roles.SingleOrDefault(x => x.Id == roleId);
        }


        //To cater for many to many (RoleUsers)
        public void AllocateRoleToUser(User u, Role r) {
            u.Roles.Add(r);
            Entity.SaveChanges();
        }


    }
}
