/**
 * Settings page interactions: global client-side search, left-nav scroll, and Phase 1 action placeholders.
 */
(function ($) {
    'use strict';

    const SECTION_ACTIVATION_OFFSET = 180;
    const SEARCH_DEBOUNCE_MS = 500;
    const HIGHLIGHT_CLASS = 'setting-item-highlight';

    const SettingsPage = {
        $page: $(),
        $search: $(),
        $emptyState: $(),
        $sections: $(),
        $navLinks: $(),
        scrollHandler: null,

        init: function () {
            this.$page = $('#settingsPage');
            if (!this.$page.length) {
                return;
            }

            this.$search = this.$page.find('#txtSearchContent');
            this.$emptyState = this.$page.find('#settingsSearchEmpty');
            this.$sections = this.$page.find('.js-setting-section');
            this.$navLinks = this.$page.find('.js-setting-nav');

            this.bindSearch();
            this.bindNavigation();
            this.bindScrollSpy();
            this.bindPlaceholderActions();
        },

        debounce: function (func, wait) {
            let timeout;
            return function () {
                const context = this;
                const args = arguments;
                clearTimeout(timeout);
                timeout = setTimeout(function () {
                    func.apply(context, args);
                }, wait);
            };
        },

        bindSearch: function () {
            const self = this;
            this.$search.on('keyup', this.debounce(function () {
                self.applySearch($(this).val());
            }, SEARCH_DEBOUNCE_MS));
        },

        applySearch: function (rawQuery) {
            const self = this;
            const query = (rawQuery || '').trim().toLowerCase();
            const $items = this.$page.find('.js-setting-item');

            if (!query) {
                $items.removeClass(HIGHLIGHT_CLASS).show();
                this.$sections.closest('.card').show();
                this.$navLinks.closest('.navi-item').show();
                this.$emptyState.addClass('d-none');
                this.scrollToSection(this.$sections.first());
                return;
            }

            $items.each(function () {
                const $item = $(this);
                const extraTerms = $item.attr('data-search-terms') || '';
                const haystack = ($item.text() + ' ' + extraTerms).toLowerCase();
                const isMatch = haystack.indexOf(query) !== -1;
                $item.toggle(isMatch);
                $item.toggleClass(HIGHLIGHT_CLASS, isMatch);
            });

            const matchingSectionIds = [];
            this.$sections.each(function () {
                const $section = $(this);
                const sectionId = $section.attr('id');
                const hasMatch = $section.find('.js-setting-item:visible').length > 0;
                $section.closest('.card').toggle(hasMatch);
                self.getNavItem(sectionId).toggle(hasMatch);

                if (hasMatch && sectionId) {
                    matchingSectionIds.push(sectionId);
                }
            });

            const hasAnyMatch = matchingSectionIds.length > 0;
            this.$emptyState.toggleClass('d-none', hasAnyMatch);

            if (hasAnyMatch) {
                this.scrollToSection($('#' + matchingSectionIds[0]));
            }
        },

        getNavItem: function (sectionId) {
            return this.$navLinks.filter('[href="#' + sectionId + '"]').closest('.navi-item');
        },

        bindNavigation: function () {
            const self = this;
            this.$navLinks.on('click', function (e) {
                e.preventDefault();
                const href = this.getAttribute('href');
                if (!href) {
                    return;
                }

                const $target = $(href);
                if ($target.length) {
                    self.scrollToSection($target);
                }
            });
        },

        bindScrollSpy: function () {
            this.scrollHandler = this.debounce(this.updateActiveSectionByScroll.bind(this), 50);
            $(window).on('scroll', this.scrollHandler);
        },

        updateActiveSectionByScroll: function () {
            const $visibleSections = this.$sections.filter(function () {
                return $(this).closest('.card').is(':visible');
            });

            const scrollTop = $(window).scrollTop();
            const windowHeight = $(window).height();
            let maxVisibleHeight = 0;
            let activeId = '';

            $visibleSections.each(function () {
                const $section = $(this);
                const offsetTop = $section.offset().top;
                const sectionHeight = $section.outerHeight();
                const visibleTop = Math.max(offsetTop, scrollTop);
                const visibleBottom = Math.min(offsetTop + sectionHeight, scrollTop + windowHeight);
                const visibleHeight = visibleBottom - visibleTop;

                if (visibleHeight > maxVisibleHeight && visibleHeight > 0) {
                    maxVisibleHeight = visibleHeight;
                    activeId = $section.attr('id');
                }
            });

            if (activeId) {
                this.setActiveNav(activeId);
            }
        },

        scrollToSection: function ($section) {
            if (!$section || !$section.length) {
                return;
            }

            const self = this;
            if (this.scrollHandler) {
                $(window).off('scroll', this.scrollHandler);
            }

            $('html, body').stop(true, false).animate({
                scrollTop: $section.offset().top - SECTION_ACTIVATION_OFFSET
            }, 500, function () {
                if (self.scrollHandler) {
                    $(window).on('scroll', self.scrollHandler);
                }
                self.setActiveNav($section.attr('id'));
            });
        },

        setActiveNav: function (sectionId) {
            this.$navLinks.removeClass('active');
            this.$navLinks.filter('[href="#' + sectionId + '"]').addClass('active');
        },

        bindPlaceholderActions: function () {
            this.$page.on('submit', '#frmSettings', function (e) {
                e.preventDefault();
            });
        }
    };

    $(function () {
        SettingsPage.init();
    });
})(jQuery);
