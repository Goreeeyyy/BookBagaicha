using BookBagaicha.Models;
using BookBagaicha.Models.Dto;
using BookBagaicha.Services;

namespace BookBagaicha.IService
{
    public interface IOrderService
    {
        // Get all orders for a user
        Task<List<OrderSummaryDto>> GetAllOrdersAsync(long userId);

        // Get the count of successful orders for a user
        Task<int> GetSuccessfulOrderCountAsync(long userId);


        // Get order details by ID
        Task<OrderDto> GetOrderDetailsAsync(Guid orderId);

<<<<<<< HEAD
        // Place a new order based on cart contents
        Task<OrderDto> PlaceOrderAsync(long userId, PlaceOrderRequest request, string claimCode);
=======


        // Place a new order based on cart items
        Task<OrderDto> PlaceOrderAsync(long userId, PlaceOrderRequest request);
>>>>>>> 17faaceed86e8d33184d627fb7213dea0f26f325

        // Cancel an existing order
        Task<bool> CancelOrderAsync(Guid orderId, long userId);

<<<<<<< HEAD
=======
        Task<OrderDto> GetOrderByClaimCodeAsync(string claimCode);
        Task<bool> CompleteOrderAsync(Guid orderId);
>>>>>>> 17faaceed86e8d33184d627fb7213dea0f26f325
    }
}