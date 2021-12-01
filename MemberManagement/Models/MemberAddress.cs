namespace MemberManagement.Models
{
    //The linking entity
    public class MemberAddress
    {
        //Composite primary key
        public int MemberID { get; set; } //foreign key for Member
        public int AdressID { get; set; } //foreign key for Address
        public int LoginID { get; set; } //foreign key for UserLogin
     
        //navigation properties
        public Member Member { get; set; }
        public Address Address { get; set; }
        public Userlogin Userlogin { get; set; }
    }
}