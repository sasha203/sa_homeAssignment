using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class LoginValidation
    {

        [Required(AllowEmptyStrings = false, ErrorMessage = "Pleased input Username")]
        public string Username {get; set;}

        [Required(AllowEmptyStrings = false, ErrorMessage = "Pleased input Password")]
        public string Password { get; set;}

        [MetadataType(typeof(LoginValidation))]
        public partial class User {
        }

    }
}
