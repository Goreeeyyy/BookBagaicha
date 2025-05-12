const token = localStorage.getItem("authToken");
console.log(token)
const notificationSocket = new WebSocket(`wss://localhost:44351/ws/notifications?token=${token}`);
notificationSocket.onopen = () => {
    console.log('[CUSTOMER] WebSocket connection established.');
};

notificationSocket.onmessage = (event) => {
    try {
        const notification = JSON.parse(event.data);
        console.log('[CUSTOMER] Received message:', notification);

        if (notification.type === 'order_confirmed') {
            console.log('[CUSTOMER] Received "order_confirmed" notification:', notification);
            displayOrderConfirmedNotification(notification.orderId, notification.orderDate, notification.claimCode, notification.totalPrice);
        }
        // ... other notification handlers ...

    } catch (error) {
        console.error('[CUSTOMER] Error parsing notification:', error);
    }
};

notificationSocket.onclose = (event) => {
    console.log('[CUSTOMER] WebSocket connection closed:', event);
};

notificationSocket.onerror = (error) => {
    console.error('[CUSTOMER] WebSocket error:', error);
};

function displayOrderConfirmedNotification(orderId, orderDate, claimCode, totalPrice) {
    const notificationArea = document.getElementById('notification-area');
    if (notificationArea) {
        const notificationElement = document.createElement('div');
        notificationElement.classList.add('notification');
        notificationElement.innerHTML = `<strong>Order Confirmed!</strong><br>Order ID: ${orderId}<br>Order Date: ${orderDate}<br>Claim Code: ${claimCode}<br>Total Price: $${totalPrice.toFixed(2)}`;
        notificationArea.prepend(notificationElement);
        console.log('[CUSTOMER] Displayed "order_confirmed" notification for order ID:', orderId);
    }
}

// ... (Rest of your general notification JavaScript) ...