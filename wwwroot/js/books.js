// This script contains only the cart and wishlist functionality
// Function to check if user is logged in
function isUserLoggedIn() {
    // Check for auth token in localStorage
    return localStorage.getItem('authToken') !== null;
}

// Function to redirect to login page
function redirectToLogin() {
    // Store the current page URL to redirect back after login
    localStorage.setItem('redirectAfterLogin', window.location.href);
    window.location.href = 'login.html';
}

// Function to add item to cart
async function addToCart(bookId) {
    try {
        // First get the cart ID
        const cartId = await getCartId();

        if (!cartId) {
            throw new Error('Could not determine cart ID');
        }

        // Add to cart using the correct endpoint
        const response = await fetch(`/api/cart/addCartItem/${cartId}/cartItems/${bookId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`
            },
            body: JSON.stringify({
                quantity: 1
            })
        });

        if (response.ok) {
            // If Toastify is available, use it, otherwise use alert
            if (typeof Toastify !== 'undefined') {
                Toastify({
                    text: "Book added to cart successfully!",
                    duration: 3000,
                    close: true,
                    gravity: "top",
                    position: "right",
                    style: {
                        background: "linear-gradient(to right, #00b09b, #96c93d)",
                    }
                }).showToast();
            } else {
                alert("Book added to cart successfully!");
            }
        } else {
            let errorMessage = "Failed to add book to cart.";
            try {
                const errorData = await response.json();
                errorMessage = errorData.message || errorMessage;
            } catch (parseError) {
                console.error("Error parsing error response:", parseError);
                // Try to get the text instead
                const errorText = await response.text();
                if (errorText) {
                    errorMessage = errorText;
                }
            }

            if (typeof Toastify !== 'undefined') {
                Toastify({
                    text: errorMessage,
                    duration: 5000,
                    close: true,
                    gravity: "top",
                    position: "right",
                    style: {
                        background: "#dc3545",
                    }
                }).showToast();
            } else {
                alert(errorMessage);
            }
        }
    } catch (error) {
        console.error('Error adding to cart:', error);
        if (typeof Toastify !== 'undefined') {
            Toastify({
                text: "An unexpected error occurred.",
                duration: 5000,
                close: true,
                gravity: "top",
                position: "right",
                style: {
                    background: "#dc3545",
                }
            }).showToast();
        } else {
            alert("An error occurred while adding to cart.");
        }
    }
}

// Function to get current user's cart ID
async function getCartId() {
    // If you store cartId in localStorage
    let cartId = localStorage.getItem('cartId');

    // If not stored, you might need to fetch it from the server
    if (!cartId) {
        try {
            const response = await fetch('/api/cart/getUserCart', {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                }
            });

            if (response.ok) {
                const data = await response.json();
                cartId = data.cartId;
                localStorage.setItem('cartId', cartId);
            }
        } catch (error) {
            console.error('Error fetching cart ID:', error);
        }
    }

    return cartId;
}

// Function to add item to wishlist
async function addToWishlist(bookId) {
    try {
        console.log("Adding to wishlist, book ID:", bookId);
        console.log("Auth token:", localStorage.getItem('authToken').substring(0, 20) + "...");

        // Use the correct endpoint as defined in your WishlistController
        const response = await fetch('/api/wishlist/add', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': `Bearer ${localStorage.getItem('authToken')}`
            },
            body: JSON.stringify({
                bookId: bookId
            })
        });

        console.log("Wishlist response status:", response.status);

        // Get response content as text first
        const responseText = await response.text();
        console.log("Raw response:", responseText);

        if (response.ok) {
            // If Toastify is available, use it, otherwise use alert
            if (typeof Toastify !== 'undefined') {
                Toastify({
                    text: "Book added to wishlist successfully!",
                    duration: 3000,
                    close: true,
                    gravity: "top",
                    position: "right",
                    style: {
                        background: "linear-gradient(to right, #00b09b, #96c93d)",
                    }
                }).showToast();
            } else {
                alert("Book added to wishlist successfully!");
            }

            // Update heart icon to show it's in wishlist
            $(`.add-to-wishlist-btn[data-book-id="${bookId}"] i`).removeClass('far').addClass('fas text-danger');
        } else {
            let errorMessage = "Failed to add book to wishlist.";

            if (responseText) {
                // Try to parse as JSON, if it fails just use the text
                try {
                    const errorData = JSON.parse(responseText);
                    errorMessage = errorData.message || errorMessage;
                } catch (e) {
                    // Not JSON, use the text directly
                    errorMessage = responseText.substring(0, 100) + "..."; // Limit length
                }
            }

            if (typeof Toastify !== 'undefined') {
                Toastify({
                    text: errorMessage,
                    duration: 5000,
                    close: true,
                    gravity: "top",
                    position: "right",
                    style: {
                        background: "#dc3545",
                    }
                }).showToast();
            } else {
                alert(errorMessage);
            }
        }
    } catch (error) {
        console.error('Error adding to wishlist:', error);
        if (typeof Toastify !== 'undefined') {
            Toastify({
                text: "An unexpected error occurred: " + error.message,
                duration: 5000,
                close: true,
                gravity: "top",
                position: "right",
                style: {
                    background: "#dc3545",
                }
            }).showToast();
        } else {
            alert("An error occurred while adding to wishlist: " + error.message);
        }
    }
}

// Function to check if books are in wishlist and update UI
async function checkWishlistStatus() {
    if (!isUserLoggedIn()) return;

    try {
        const bookIds = $('.add-to-wishlist-btn').map(function () {
            return $(this).data('book-id');
        }).get();

        for (const bookId of bookIds) {
            const response = await fetch(`/api/wishlist/check/${bookId}`, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('authToken')}`
                }
            });

            if (response.ok) {
                const data = await response.json();
                if (data.isInWishlist) {
                    $(`.add-to-wishlist-btn[data-book-id="${bookId}"] i`).removeClass('far').addClass('fas text-danger');
                }
            }
        }
    } catch (error) {
        console.error('Error checking wishlist status:', error);
    }
}

// Set up the cart and wishlist button event handlers
function setupCartWishlistButtons() {
    // Add to cart button
    $('.add-to-cart-btn').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();

        if (!isUserLoggedIn()) {
            redirectToLogin();
            return;
        }

        // If user is logged in, proceed with adding to cart
        const bookId = $(this).data('book-id');
        addToCart(bookId);
    });

    // Add to wishlist button
    $('.add-to-wishlist-btn').on('click', function (e) {
        e.preventDefault();
        e.stopPropagation();

        if (!isUserLoggedIn()) {
            redirectToLogin();
            return;
        }

        // If user is logged in, proceed with adding to wishlist
        const bookId = $(this).data('book-id');
        addToWishlist(bookId);
    });
}

// When the document is ready, set up the buttons
$(document).ready(function () {
    // If buttons already exist on the page, set them up
    if ($('.add-to-cart-btn').length > 0 || $('.add-to-wishlist-btn').length > 0) {
        setupCartWishlistButtons();
    }

    // If user is logged in, check wishlist status
    if (isUserLoggedIn()) {
        checkWishlistStatus();
    }
});