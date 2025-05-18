const notificationSocket = new WebSocket("wss://localhost:44351/ws/notifications");
notificationSocket.onopen = () => {
    console.log('[STAFF] WebSocket connection established.');
};

notificationSocket.onmessage = (event) => {
    try {
        const notification = JSON.parse(event.data);
        console.log('[STAFF] Received message:', notification);

        if (notification.type === 'new_order') {
            console.log('[STAFF] Received "new_order" notification:', notification);
            displayNewOrderNotificationForStaff(notification.orderId, notification.orderDate, notification.customerUserId, notification.totalPrice);
        }
        // ... other notification handlers ...

    } catch (error) {
        console.error('[STAFF] Error parsing notification:', error);
    }
};

notificationSocket.onclose = (event) => {
    console.log('[STAFF] WebSocket connection closed:', event);
};

notificationSocket.onerror = (error) => {
    console.error('[STAFF] WebSocket error:', error);
};

function displayNewOrderNotificationForStaff(orderId, orderDate, customerUserId, totalPrice) {
    const staffNotificationArea = document.getElementById('staff-notification-area');
    if (staffNotificationArea) {
        const notificationElement = document.createElement('div');
        notificationElement.classList.add('notification-staff');
        notificationElement.innerHTML = `<strong>New Order Received!</strong><br>Order ID: ${orderId}<br>Order Date: ${orderDate}<br>Customer ID: ${customerUserId}<br>Total Price: Rs ${totalPrice.toFixed(2)}`;
        staffNotificationArea.prepend(notificationElement);
        console.log('[STAFF] Displayed "new_order" notification for order ID:', orderId);
    }
}

