document.addEventListener('DOMContentLoaded', () => {
    // Sample book data
    const books = [
        {
            title: "Thunmanhandiya",
            author: "Mahagamasakara",
            price: 790,
            genre: "fiction",
            image: "Images/default.jpg"
        },
        // Add more books as needed
    ];

    // Initialize cart
    let cart = JSON.parse(localStorage.getItem('cart')) || [];
    updateCartCount();

    // Render books
    renderBooks(books);

    // Search functionality
    document.getElementById('searchInput').addEventListener('input', (e) => {
        const term = e.target.value.toLowerCase();
        const filtered = books.filter(book =>
            book.title.toLowerCase().includes(term) ||
            book.author.toLowerCase().includes(term)
        );
        renderBooks(filtered);
    });

    // Filter/Sort handlers
    document.querySelectorAll('[data-sort], [data-genre]').forEach(item => {
        item.addEventListener('click', handleFilter);
    });

    function renderBooks(booksArray) {
        const grid = document.getElementById('bookGrid');
        grid.innerHTML = booksArray.map(book => `
            <div class="col">
                <div class="card h-100 text-center p-2">
                    <a href="book-details.html" class="stretched-link"></a>
                    <img src="${book.image}" 
                         class="card-img-top book-img" 
                         alt="${book.title} Cover">
                    <div class="card-body">
                        <h6 class="card-title mb-1">${book.title}</h6>
                        <p class="card-subtitle text-muted small mb-2">${book.author}</p>
                        <p class="mb-2 text-dark fw-bold">Rs. ${book.price}/-</p>
                        <div class="d-flex justify-content-center gap-2">
                            <button class="btn btn-sm btn-outline-primary add-to-cart">
                                Add to Cart
                            </button>
                            <button class="btn btn-sm btn-outline-secondary wishlist">
                                <i class="far fa-heart"></i>
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        `).join('');

        // Add event listeners to new buttons
        document.querySelectorAll('.add-to-cart').forEach(btn => {
            btn.addEventListener('click', addToCart);
        });
    }

    function addToCart(e) {
        const card = e.target.closest('.card');
        const title = card.querySelector('.card-title').textContent;
        const price = card.querySelector('.fw-bold').textContent;

        cart.push({ title, price });
        localStorage.setItem('cart', JSON.stringify(cart));
        updateCartCount();
    }

    function updateCartCount() {
        document.getElementById('cartCount').textContent = cart.length;
    }

    function handleFilter(e) {
        // Implement filter/sort logic here
    }
});
</html>