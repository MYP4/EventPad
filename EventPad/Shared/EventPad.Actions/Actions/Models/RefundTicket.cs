﻿namespace EventPad.Actions;

public class RefundTicket
{
    public Guid UserAccountID { get; set; }
    public Guid EventAccountID { get; set; }
    public Guid Ticket { get; set; }
    public float Amount { get; set; }
}