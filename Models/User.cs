using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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

    [JsonIgnore]
    public List<Subscription> Subscriptions { get; set; }

    [JsonIgnore]
    [BsonIgnore] //Ignorowane przez nosql, ponieważ posty nie znajdują się w tej samej kolekcji co użytkownicy, więc nie można ich wyciągnąć razem, przechowana jest tylko referencja, czyli id. 
    public List<Post> ViewedPosts { get; set; }

    [JsonIgnore]
    [NotMapped] //Ignorowane przez sql, służy przechowaniu referencji (id) w implementacji nosql, która opcjonalnie posłuży do wyciągnięcia danych w kolejnym zapytaniu.
    public List<string> ViewedPostsIds { get; set; }

    [JsonIgnore]
    [NotMapped]
    public List<Rating> Ratings { get; set; }

    [JsonIgnore]
    [BsonIgnore] 
    public Channel Channel { get; set; }

    [JsonIgnore]
    [BsonIgnore] 
    public List<Message> SendMessages { get; set; }

    [JsonIgnore]
    [BsonIgnore] 
    public List<Message> ReceivedMessages { get; set; }

    [JsonIgnore]
    [BsonIgnore]
    public List<Comment> Comments { get; set; }
}
