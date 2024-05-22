using System;
using System.Collections.Generic;
using Xunit;
using Moq;
using Biljettshoppen;

namespace Biljettshoppen.Tests
{
    public class BookingManagerTests
    {
        private Mock<IEventManager> mockEventManager;
        private BookingManager bookingManager;

        public BookingManagerTests()
        {
            mockEventManager = new Mock<IEventManager>();
            bookingManager = new BookingManager("test_bookings.json", mockEventManager.Object);
        }

        [Fact]
        public void CreateBooking_ValidDetails_ShouldCreateBooking()
        {
            // Arrange
            var eventID = 1;
            var seatIDs = new List<int> { 1, 2, 3 };
            var selectedEvent = new Event
            {
                EventID = eventID,
                AvailableSeats = new List<int> { 1, 2, 3, 4, 5 },
                UnavailableSeats = new List<int>()
            };

            mockEventManager.Setup(em => em.GetEventById(eventID)).Returns(selectedEvent);

            // Act
            var booking = bookingManager.CreateBooking("John Doe", "john@example.com", eventID, seatIDs, "Credit Card");

            // Assert
            Assert.NotNull(booking);
            Assert.Equal("John Doe", booking.Name);
            Assert.Equal("john@example.com", booking.Email);
            Assert.Equal(eventID, booking.EventID);
            Assert.Equal(seatIDs, booking.SeatIDs);
        }

        [Fact]
        public void CreateBooking_ExceedSeatLimit_ShouldNotCreateBooking()
        {
            // Arrange
            var eventID = 1;
            var seatIDs = new List<int> { 1, 2, 3, 4, 5, 6 };
            var selectedEvent = new Event
            {
                EventID = eventID,
                AvailableSeats = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                UnavailableSeats = new List<int>()
            };

            mockEventManager.Setup(em => em.GetEventById(eventID)).Returns(selectedEvent);

            // Act
            var booking = bookingManager.CreateBooking("John Doe", "john@example.com", eventID, seatIDs, "Credit Card");

            // Assert
            Assert.Null(booking);
        }

        [Fact]
        public void CancelBooking_ValidBookingID_ShouldCancelBooking()
        {
            // Arrange
            var eventID = 1;
            var seatIDs = new List<int> { 1, 2, 3 };
            var selectedEvent = new Event
            {
                EventID = eventID,
                AvailableSeats = new List<int> { 4, 5, 6 },
                UnavailableSeats = new List<int> { 1, 2, 3 }
            };

            var booking = new Booking
            {
                BookingID = 1,
                Name = "John Doe",
                Email = "john@example.com",
                EventID = eventID,
                SeatIDs = seatIDs,
                PaymentMethod = "Credit Card"
            };

            bookingManager.CreateBooking("John Doe", "john@example.com", eventID, seatIDs, "Credit Card");

            mockEventManager.Setup(em => em.GetEventById(eventID)).Returns(selectedEvent);

            // Act
            var result = bookingManager.CancelBooking(1);

            // Assert
            Assert.True(result);
        }
    }

    public class EventManagerTests
    {
        private EventManager eventManager;

        public EventManagerTests()
        {
            eventManager = new EventManager("test_events.json");
        }

        [Fact]
        public void CreateEvent_ValidDetails_ShouldCreateEvent()
        {
            // Act
            var newEvent = eventManager.CreateEvent("Concert", "18:00", "2024-12-01", "Stadium");

            // Assert
            Assert.NotNull(newEvent);
            Assert.Equal("Concert", newEvent.EventName);
            Assert.Equal("18:00", newEvent.Time);
            Assert.Equal("2024-12-01", newEvent.Date);
            Assert.Equal("Stadium", newEvent.Venue);
        }

        [Fact]
        public void RemoveEvent_ValidEventID_ShouldRemoveEvent()
        {
            // Arrange
            var newEvent = eventManager.CreateEvent("Concert", "18:00", "2024-12-01", "Stadium");

            // Act
            var result = eventManager.RemoveEvent(newEvent.EventID);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void EventExists_ValidEventID_ShouldReturnTrue()
        {
            // Arrange
            var newEvent = eventManager.CreateEvent("Concert", "18:00", "2024-12-01", "Stadium");

            // Act
            var exists = eventManager.EventExists(newEvent.EventID);

            // Assert
            Assert.True(exists);
        }
    }
}
