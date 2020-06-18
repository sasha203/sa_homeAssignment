using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using DataAccess;


namespace BusinessLogic
{
    public class UsersBL
    {

        public bool Login(string user, string pass) {
            UsersRepository ur = new UsersRepository();
            return ur.Login(user, Encryption.HashPassword(pass));
        }

        public void Register(User u) {
            UsersRepository ur = new UsersRepository();
            RolesRepository rr = new RolesRepository();
            
            ur.Entity = rr.Entity; //The memory location of ur is equall to the memory location of rr (is the same object.)  will keep 1 memory location.
              
            u.Id = Guid.NewGuid();
            u.Password = Encryption.HashPassword(u.Password);

            //Generating keys into db
            var myKeys = Encryption.GenerateAsymmetricKeys();
            u.PublicKey = myKeys.PublicKey;
            u.PrivateKey = myKeys.PrivateKey;

            ur.AddUser(u);

           
            var r = rr.GetRole("User");
            //var r = rr.GetRole("Admin");
            rr.AllocateRoleToUser(u, r);

        }

        public User GetUser(string user)
        {
            return new UsersRepository().GetUser(user);
        }

        public User GetUserById(Guid id) {
            return new UsersRepository().GetUserById(id);
        }

        public User GetEmail(string email) {
            return new UsersRepository().GetEmail(email);
        }


        //Register Validation.
        public bool UserAlreadyExists(string user)
        {
            if (GetUser(user) == null)
            {
                return false;  //returns false if username doesn't match in db.
            }
            return true; //if user exists in db.
        }

        public bool EmailAlreadyExists(string email)
        {
            if (GetEmail(email) == null)
            {
                return false;  
            }
            return true; //if email exists in db.
        }

        //---------------------------------------------

        public bool IsUserBlocked(string username) {
            return GetUser(username).Blocked;
        }

        public int NumOfAttemps(string username) {
            return GetUser(username).NoOfAttemps;
        }

        public void IncreaseAttemps(string Username) {
            UsersRepository ur = new UsersRepository();
            ur.IncreaseAttemps(Username);
        }

        public void ResetAttemps(string Username) {
            UsersRepository ur = new UsersRepository();
            ur.ResetAttemps(Username);
        }

        public void BlockUser(string Username) {
            UsersRepository ur = new UsersRepository();
            ur.BlockUser(Username);
        }




        public IQueryable<Role> GetRolesOfUser(string username) { //jekk naqleb al Ilist min naha lohra ma ninsiex naqliba min awn.

        return new UsersRepository().GetRolesOfUser(username);
    }
        




    }
}
