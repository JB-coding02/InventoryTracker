// Dark Mode Toggle Functionality

document.addEventListener('DOMContentLoaded', function() {
    // Initialize dark mode from localStorage
    const isDarkMode = localStorage.getItem('dark-mode') === 'true';

    if (isDarkMode) {
        document.body.classList.add('dark-mode');
        updateToggleButton();
    }

    // Add click handler to toggle button
    const toggleButton = document.getElementById('dark-mode-toggle');
    if (toggleButton) {
        toggleButton.addEventListener('click', function() {
            toggleDarkMode();
        });
    }
});

function toggleDarkMode() {
    const body = document.body;
    body.classList.toggle('dark-mode');

    // Save preference to localStorage
    const isDarkMode = body.classList.contains('dark-mode');
    localStorage.setItem('dark-mode', isDarkMode);

    // Update button appearance
    updateToggleButton();
}

function updateToggleButton() {
    const button = document.getElementById('dark-mode-toggle');
    const isDarkMode = document.body.classList.contains('dark-mode');

    if (button) {
        if (isDarkMode) {
            button.textContent = '☀️ Light Mode';
            button.setAttribute('aria-label', 'Switch to light mode');
        } else {
            button.textContent = '🌙 Dark Mode';
            button.setAttribute('aria-label', 'Switch to dark mode');
        }
    }
}
