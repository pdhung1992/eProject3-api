using System.ComponentModel.DataAnnotations.Schema;

namespace web_api.Entities;

public class Order
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    
    public string EventName { get; set; }
    
    public string DeliveryAddress { get; set; }
    
    public DateTime DeliveryDate { get; set; }
    
    public string DeliveryPerson { get; set; }
    
    public string DeliveryPhone { get; set; }
    
    public string AdditionalRequirement { get; set; }
    
    public double UnitPrice { get; set; }
    
    public int Quantity { get; set; }
    
    public string PayMethod { get; set; }
    
    public double PaidAmount { get; set; }
    
    public string PaidStatus { get; set; }
    
    public string OrderStatus { get; set; }
}