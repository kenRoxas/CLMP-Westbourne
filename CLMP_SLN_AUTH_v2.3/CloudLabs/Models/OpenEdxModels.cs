
namespace CloudSwyft.CloudLabs.Models
{
    public class OpenEdxUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool isDeleted { get; set; }
        public bool isDisabled { get; set; }
        public string CreatedBy { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string EmailConfirmed { get; set; }
    }
    public class VeProfile
    {
        public int VEProfileID { get; set; }
        public int CourseID { get; set; }
        public string Name { get; set; }

    }
}