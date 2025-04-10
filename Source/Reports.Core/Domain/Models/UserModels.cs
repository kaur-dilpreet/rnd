using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Reports.Core.Domain.Models
{
    public class UsersModel
    {
        public UsersModel()
        {
            this.SearchModel = new UsersSearchModel();
        }

        public IEnumerable<Core.Domain.Models.UserListModel> Users { get; set; }
        public Core.Domain.Models.UsersSearchModel SearchModel { get; set; }
        public PaginationModel PaginationModel { get; set; }
    }

    public class UserModel
    {
        public Int64 Id { get; set; }
        public Guid UniqueId { get; set; }
        public String Email { get; set; }
        public String FullName { get; set; }
        public Boolean AskBIAccess { get; set; }
        public Boolean CMOChatAccess { get; set; }
        public Boolean SDRAIAccess { get; set; }
        public Boolean ChatGPIAccess { get; set; }
        public String NTID { get; set; }
    }

    public class UserListModel : UserModel
    {
        public Byte UserRole { get; set; }
        public DateTime? LastActivityDateTime { get; set; }
    }

    public class UserUpdateModel
    {
        [Required]
        public Int64 Id { get; set; }
        public String Email { get; set; }
        public String FullName { get; set; }

        [Display(Name = "User Role")]
        [Required(ErrorMessage = "User role is required.")]
        public Byte UserRoleId { get; set; }

        [Display(Name = "Disable Default Profile")]
        public Boolean DisableDefaultProfile { get; set; }

        public IEnumerable<KeyValuePair<Byte, String>> UserRoles { get; set; }
    }

    public class UserAddModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required.")]
        [StringLength(256, ErrorMessage = "Maximum 256 characters are allowed.")]
        [RegularExpression(Core.Domain.Enumerations.EmailAddressRegEx, ErrorMessage = "Invalid email address.")]
        public String Email { get; set; }

        [Display(Name = "User Role")]
        [Required(ErrorMessage = "User role is required.")]
        public Byte UserRoleId { get; set; }

        [Display(Name = "Disable Default Profile")]
        public Boolean DisableDefaultProfile { get; set; }

        public IEnumerable<KeyValuePair<Byte, String>> UserRoles { get; set; }
    }

    public class UserAssignProfileModel
    {
        [Required]
        public Int64 Id { get; set; }
        public String Email { get; set; }
        public String FullName { get; set; }
        public List<Int64> UserProfileIds { get; set; }
        public IEnumerable<KeyValuePair<Int64, String>> Profiles { get; set; }
    }

    public class UsersSearchModel
    {
        public UsersSearchModel()
        {
            this.SearchCriteria = String.Empty;
        }

        [Display(Name = "Search")]
        [StringLength(256, ErrorMessage = "Maximum 256 characters are allowed.")]
        public String SearchCriteria { get; set; }
    }

    public class UsersAddBulkModel
    {
        public UsersAddBulkModel()
        {
            this.Users = new List<Models.UserAddBulkModel>();
        }

        public List<UserAddBulkModel> Users { get; set; }

        [Required]
        public String FileName { get; set; }
    }

    public class UserAddBulkModel
    {
        public String Email { get; set; }
        public Boolean IsValid { get; set; }
        public Boolean UserAlreadyExists { get; set; }
        public Boolean DuplicateUser { get; set; }
        public Boolean InvalidEmail { get; set; }
    }
}
