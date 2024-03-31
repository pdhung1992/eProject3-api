namespace web_api.DTOs;

public class ReportDTO
{
    public int Id { get; set; }
    
    public string InvoiceNumber { get; set; }
    
    public DateTime OrderDate { get; set; }
    
    public string Status { get; set; }
    
    public double PrePaid { get; set; }
    
    public double TotalAmount { get; set; }
}