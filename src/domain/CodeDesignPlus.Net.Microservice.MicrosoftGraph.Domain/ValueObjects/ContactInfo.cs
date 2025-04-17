using System;

namespace CodeDesignPlus.Net.Microservice.MicrosoftGraph.Domain.ValueObjects;


public sealed partial class ContactInfo
{
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? Country { get; private set; }
    public string? ZipCode { get; private set; }
    public string? Phone { get; private set; }
    public string[] Email { get; private set; } = [];

    public ContactInfo()
    {
    }

    public ContactInfo(string address, string city, string state, string country, string zipCode, string phone, string[] email)
    {
        Address = address;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
        Phone = phone;
        Email = email;
    }

    public static ContactInfo Create(string address, string city, string state, string country, string zipCode, string phone, string[] email)
    {
        return new ContactInfo(address, city, state, country, zipCode, phone, email);
    }
}
