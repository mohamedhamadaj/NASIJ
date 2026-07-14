document.addEventListener('DOMContentLoaded', () => {
    const sidebar = document.getElementById('adminSidebar');
    const toggleBtn = document.getElementById('sidebarToggle');
    const themeToggleBtn = document.getElementById('themeToggle');
    const rootHtml = document.documentElement;

    // 1. Sidebar Collapse/Expand Toggle
    if (sidebar && toggleBtn) {
        toggleBtn.addEventListener('click', () => {
            sidebar.classList.toggle('collapsed');
            
            // Save state to localStorage
            const isCollapsed = sidebar.classList.contains('collapsed');
            localStorage.setItem('adminSidebarCollapsed', isCollapsed);
        });

        // Restore sidebar state on load
        const savedSidebarState = localStorage.getItem('adminSidebarCollapsed');
        if (savedSidebarState === 'true') {
            sidebar.classList.add('collapsed');
        }
    }

    // 2. Light/Dark Theme Switcher
    // Default theme is dark
    const savedTheme = localStorage.getItem('adminTheme') || 'dark';
    rootHtml.setAttribute('data-theme', savedTheme);
    updateThemeIcon(savedTheme);

    if (themeToggleBtn) {
        themeToggleBtn.addEventListener('click', () => {
            const currentTheme = rootHtml.getAttribute('data-theme');
            const newTheme = currentTheme === 'dark' ? 'light' : 'dark';
            
            rootHtml.setAttribute('data-theme', newTheme);
            localStorage.setItem('adminTheme', newTheme);
            updateThemeIcon(newTheme);
        });
    }

    function updateThemeIcon(theme) {
        if (!themeToggleBtn) return;
        const icon = themeToggleBtn.querySelector('i');
        if (icon) {
            if (theme === 'dark') {
                icon.className = 'fas fa-sun';
            } else {
                icon.className = 'fas fa-moon';
            }
        }
    }

    // 3. Simple search animation/functionality
    const searchInput = document.querySelector('.search-box input');
    if (searchInput) {
        searchInput.addEventListener('keydown', (e) => {
            if (e.key === 'Enter') {
                alert(`Searching inventory for: "${searchInput.value}"`);
            }
        });
    }
});
