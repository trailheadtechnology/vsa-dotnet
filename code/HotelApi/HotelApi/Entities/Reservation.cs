namespace Hotel.Api.Entities;

public class Reservation
{
    public Guid Id { get; set; }
    public int RoomId { get; set; }
    public string GuestFirstName { get; set; }
    public string GuestLastName { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
}
