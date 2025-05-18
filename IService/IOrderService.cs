using BookBagaicha.Models;
using BookBagaicha.Models.Dto;

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



        // Place a new order based on cart items
        Task<OrderDto> PlaceOrderAsync(long userId, PlaceOrderRequest request);

        // Cancel an existing order
        Task<bool> CancelOrderAsync(Guid orderId, long userId);

        Task<OrderDto> GetOrderByClaimCodeAsync(string claimCode);
        Task<bool> CompleteOrderAsync(Guid orderId);
    }
}