namespace PlantDiaganoseDisease.Models.RequestModels
{
    public class UserMasterReqModel
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public List<UserRole>? RoleList { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Address { get; set; }
        public string? Flag { get; set; }
     //   public string? UserStatus { get; set; }
        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; } = 0;
    }
    public class UserRole
    {
        public int UserId { get; set; } = 0;
        public int RoleId { get; set; } = 0;
        public string? RoleName { get; set; }
        public bool IsActive { get; set; } = false;
    }
}
