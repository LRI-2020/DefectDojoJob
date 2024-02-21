namespace DefectDojoJob.Models.DefectDojo;

public class User
{
    public User(string userName)
    {
        UserName = userName;
    }

    public int Id { get; set; }
    public string UserName { get; set; }
}