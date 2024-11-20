using Microsoft.EntityFrameworkCore;
using BDwAS_projekt.Models;

namespace BDwAS_projekt.Data;

//Interfejs który muszą implementować wszystkie bazy danych.
public interface IDbContext
{
    public List<User> GetUsers(); //Zwraca tylko użytkowników, nie musi zwracać subskrypcji, Payments, ViewedPosts, Channel
    public User GetUser(string userId); //Zwraca pełnego użytkownika z subskrypcjami, Payments, ViewedPosts, Channel
    public bool AddUser(User user);
    public bool DeleteUser(string userId);
    public bool UpdateUser(User user);
    public bool VerifyUser(string userId);

    public bool AddSubscription(Subscription subscription, string userId);
    public bool AddPayment(Payment payment, string subscriptionId, string userId);

    public List<Channel> GetChannels();
    //...
}