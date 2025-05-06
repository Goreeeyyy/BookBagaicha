$(document).ready(function () {
    // Initialize dropdown functionality
    $('.dropdown-toggle').dropdown();

    // Card hover effect
    $('.card').hover(
        function () {
            $(this).css('transform', 'translateY(-5px)');
        },
        function () {
            $(this).css('transform', 'translateY(0)');
        }
    );
});