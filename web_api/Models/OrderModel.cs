namespace web_api.Models;

public class OrderModel
{
    public int UserId { get; set; }
    
    public int RestaurantId { get; set; }
    
    public string EventName { get; set; }
    
    public string DeliveryAddress { get; set; }
    
    public DateTime DeliveryDate { get; set; }
    
    public string DeliveryPerson { get; set; }
    
    public string DeliveryPhone { get; set; }
    
    public string AdditionalRequirement { get; set; }
    
    public double UnitPrice { get; set; }
    
    public double ComboDiscount { get; set; }
    
    public int Quantity { get; set; }
    
    public double TotalAmount { get; set; }
    
    public double PrePaid { get; set; }
    
    public int StatusId { get; set; }

    public List<OrderDetailModel> Details { get; set; }
}