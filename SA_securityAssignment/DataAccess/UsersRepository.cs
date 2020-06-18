using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace DataAccess
{
    public class UsersRepository : ConnectionClass
    {
        public UsersRepository() : base() {
        }

        // Returns true if user and pass are correct
        public bool Login(string user, string pass) {
            if (Entity.Users.SingleOrDefault(x => x.Username == user && x.Password == pass) != null) {
                return true;
            }
            return false;
        }

        public void AddUser(User u) { 
            Entity.Users.Add(u);
            Entity.SaveChanges();
        }

        public User GetUser(string user)
        {
            return Entity.Users.SingleOrDefault(x => x.Username == user);
        }

        public User GetUserById(Guid id) {
            return Entity.Users.SingleOrDefault(x => x.Id == id);
        }


        public User GetEmail(string email) {
            return Entity.Users.SingleOrDefault(x => x.Email == email);
        }

        public void IncreaseAttemps(string Username) {
            GetUser(Username).NoOfAttemps += 1;
            Entity.SaveChanges();
        }

        public void BlockUser(string Username) {
            GetUser(Username).Blocked = true;
            Entity.SaveChanges();
        }

        public void ResetAttemps(string Username) {
            GetUser(Username).NoOfAttemps = 0;
            Entity.SaveChanges();
        }


        public IQueryable<Role> GetRolesOfUser(string username)
        { 
            /* 
                Select Roles.Id, Roles.Title from Roles inner join UserRoles on UserRoles.RoleId = Roles.Id
                inner join Users on UserRoles.userId = Users.Id
                Where Users.Username = username

                is the same as the return line of code.
            */

            return Entity.Users.SingleOrDefault(x => x.Username == username).Roles.AsQueryable();
            //return Entity.Users.SingleOrDefault(x => x.Username == username).Roles.ToList(); jekk namila IList ir return type.
        }


    }
}
