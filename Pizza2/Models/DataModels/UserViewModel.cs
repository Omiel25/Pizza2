using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using System.Text;

namespace Pizza2.Models
{
    public class UserViewModel
    {
        [Key]
        public int Id { get; set; }
        public int Privilages { get; set; }

        [Column(TypeName = "VARCHAR")]
        [StringLength(20)]
        public string UserName { get; set; }

        public string Password {
            get => _password;
            set
            {
                if(value != null)
                {
                    SHA256 sha256 = SHA256.Create();

                    byte[] hashData = sha256.ComputeHash(Encoding.Default.GetBytes(value));
                    StringBuilder returnValue = new StringBuilder();

                    for (int i = 0; i < hashData.Length; i++)
                    {
                        returnValue.Append(hashData[i].ToString());
                    }

                    _password = returnValue.ToString();
                }
                
            }
        }

        private string _password;

        public DateTime? CreationDate { get; set; }

        public UserViewModel()
        {

        }

        public bool ComparePasswords(string pswd)
        {
            //Testing method
            SHA256 sha256 = SHA256.Create();

            byte[] hashData = sha256.ComputeHash(Encoding.Default.GetBytes(pswd));
            StringBuilder returnValue = new StringBuilder();

            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }

            if(Password == returnValue.ToString())
            {
                return true;
            } else {

                return false;
            }
        }

    }
}
