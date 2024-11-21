using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BDwAS_projekt.Models;

public class User()
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [Key]
    public string Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public DateTime RegistrationDate { get; set; }

    public Boolean IsVerified { get; set; }

    public List<Subscription> Subscriptions { get; set; }

    [BsonIgnore] //Ignorowane przez nosql, ponieważ posty nie znajdują się w tej samej kolekcji co użytkownicy, więc nie można ich wyciągnąć razem, przechowana jest tylko referencja, czyli id. 
    public List<Post> ViewedPosts { get; set; }

    [NotMapped] //Ignorowane przez sql, służy przechowaniu referencji (id) w implementacji nosql, która opcjonalnie posłuży do wyciągnięcia danych w kolejnym zapytaniu.
    public List<string> ViewedPostsIds { get; set; }

    [NotMapped]
    public List<Rating> Ratings { get; set; }

    [BsonIgnore] 
    public Channel Channel { get; set; }

    [BsonIgnore] 
    public List<Message> SendMessages { get; set; }

    [BsonIgnore] 
    public List<Message> ReceivedMessages { get; set; }

    [BsonIgnore]
    public List<Comment> Comments { get; set; }
}
