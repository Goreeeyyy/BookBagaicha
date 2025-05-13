window.updateCartBadge = async function () {
    try {
        const authToken = localStorage.getItem('authToken');
        if (!authToken) return;

        const response = await fetch('/api/cart/getUserCart', {
            method: 'GET',
            headers: { 'Authorization': `Bearer ${authToken}` }
        });

        if (!response.ok) return;

        const cart = await response.json();
        const cartId = cart.cartId;

        const itemsResponse = await fetch(`/api/cart/getAllCartItems/${cartId}/cartItems`, {
            method: 'GET',
            headers: { 'Authorization': `Bearer ${authToken}` }
        });

        if (!itemsResponse.ok) return;

        const cartData = await itemsResponse.json();
        const items = cartData.items?.$values || cartData.items || [];
        const itemCount = items.reduce((sum, item) => sum + (item.quantity || 1), 0);

        const cartIcon = document.querySelector('.fa-shopping-cart');
        if (cartIcon) {
            let badge = cartIcon.parentElement.querySelector('.cart-badge');
            if (!badge) {
                badge = document.createElement('span');
                badge.classList.add('cart-badge');
                badge.style.cssText = `
                    position: absolute;
                    top: -5px;
                    right: -8px;
                    background-color: red;
                    color: white;
                    border-radius: 50%;
                    font-size: 0.7rem;
                    padding: 2px 6px;
                    line-height: 1;
                `;
                cartIcon.parentElement.style.position = 'relative';
                cartIcon.parentElement.appendChild(badge);
            }

            if (itemCount > 0) {
                badge.textContent = itemCount;
                badge.style.display = 'block';
            } else {
                badge.style.display = 'none';
            }
        }
    } catch (err) {
        console.error('Error updating cart badge:', err);
    }
};

// Initialize on page load
document.addEventListener('DOMContentLoaded', function () {
    window.updateCartBadge();
});