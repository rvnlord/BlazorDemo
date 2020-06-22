/// <reference path="../lib/libman/jquery/jquery.js" />
/// <reference path="../lib/libman/bootstrap/js/bootstrap.bundle.js" />
/// <reference path="../lib/libman/jqueryui/jquery-ui.js" />

function uuidv4() {
    return "xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx".replace(/[xy]/g, function (c) {
        const r = Math.random() * 16 | 0;
        const v = c === "x" ? r : r & 0x3 | 0x8;
        return v.toString(16);
    });
}

function setEmptyClassForEmptyElements() {
    const $divs = $("div").toArray().map(div => $(div));
    for (let $div of $divs) {
        if ($div.text().replace(/\s\s+/g, " ").trim().length === 0 || $div.children().length === 0) {
            $div.addClass("empty");
        } else {
            $div.removeClass("empty");
        }
    }
}

$(document).ready(function () {

    // #region BOOSTRAP DOM CHANGES

    const MutationObserver = window.MutationObserver || window.WebKitMutationObserver;
    const observer = new MutationObserver(onDomChanged);
    observer.observe(document, { subtree: true, attributes: true });

    var changingDomInProgress = false;
    function onDomChanged(mutations, o) {

        if (changingDomInProgress)
            return;

        changingDomInProgress = true;

        //console.log(mutations, o);

        for (let $fc of $.makeArray($(".form-control")).map(fc => $(fc))) {
            if ($fc.prev() && $fc.prev().hasClass("input-group-prepend")) {
                const $prependGroup = $fc.prev();
                const disabledClass = "input-group-prepend-input-group-text-disabled";
                const $igts = $.makeArray($prependGroup.children(".input-group-text")).map(igt => $(igt));

                for (let $igt of $igts) {
                    if (!$fc.prop("disabled") && $igt.hasClass(disabledClass)) {
                        $igt.removeClass(disabledClass);
                    } else if ($fc.prop("disabled") && !$igt.hasClass(disabledClass)) {
                        $igt.addClass(disabledClass);
                    }
                }
            }
        }

        const $navLinks = $.makeArray($(".dropdown-menu li a.nav-link")).map(a => $(a));

        for (let $navLink of $navLinks) {
            const $ddlNavLink = $navLink.closest("li").closest(".nav-item.dropdown").children(".nav-link").first();
            const $navItem = $navLink.closest("li");

            if ($navLink.is(".active")) {
                $navItem.addClass("active");
                $ddlNavLink.addClass("active");
            } else {
                $navItem.removeClass("active");
            }
        }

        createDdlArrowIfRequired();
        setEmptyClassForEmptyElements();
        makeDataPickers();

        changingDomInProgress = false;
    }

    // #endregion

    // #region BOOTSTRAP WINDOW RESIZES

    $(window).on("resize", e => {

        setEmptyClassForEmptyElements();
        positionDatePickers();

    });

    // #endregion

    // #region BOOTSTRAP BUTTON

    $(document).on("mouseenter", ".btn", e => {
        var $btn = $(e.target).is(".btn") ? $(e.target) : $(e.target).closest(".btn");
        var $appendGroup = $btn.closest(".input-group-append, .input-group-prepend");
        if ($appendGroup[0]) {
            const $otherBtns = $.makeArray($(".btn").not($btn)).map(s => $(s));
            const zIndex = Math.min.apply(null, $otherBtns.map($b => parseInt($b.css("z-index"))).filter(v => !Number.isNaN(v)));

            for (let $oBtn of $otherBtns) {
                $oBtn.css("z-index", zIndex);
            }

            $btn.css("z-index", zIndex + 1);
        }
    });

    // #endregion

    // #region BOOTSTRAP INPUT

    $(document).on("change", "input.form-control", function() {
        setEmptyClassForEmptyElements();
    });

    // #endregion

    // #region BOOTSTRAP DATE PICKER

    window.makeDataPickers = () => { // called on dom change
        const $dps = $("input[type='date']").toArray().map(dp => $(dp)).filter($dp => $dp.css("display") !== "none");
        for (let $dp of $dps) {
            $dp.css("display", "none");
            const val = $dp.val().split("-").reverse().join("-");
            const $tdp = $(`<input class='${$dp.attr("class")}' type='text' value='${val}' />`);
            $tdp.insertAfter($dp);
            $tdp.datepicker({
                dateFormat: "dd-mm-yy"
            });
            $tdp.datepicker("setDate", val);

            if (!$tdp.next(".input-group-append")[0] && !$tdp.next(".my-dp-icon")[0]) {
                const right = `${parseFloat($tdp.parent().css("padding-right")) + parseFloat($tdp.css("padding-right"))}px`;
                const top = `${$tdp.position().top}px`;
                const height = `${$tdp.outerHeight(false)}px`;
                const $icon = $(`<i class='fa fa-calendar-alt my-dp-icon' style='position: absolute; right: ${right}; top: ${top}; height: ${height}'></i>`);
                $icon.insertAfter($tdp);
            }
        }
    }

    window.positionDatePickers = () => { // called on resize
        const $tdps = $("input[type='date']").toArray().map(dp => $(dp)).filter($dp => $dp.css("display") === "none").map($dp => $dp.next());
        for (let $tdp of $tdps) {
            const $uiDp = $tdp.datepicker("widget");
            $uiDp.css({
                top: ($tdp.offset().top + $tdp.outerHeight()) + "px", 
                left: $tdp.offset().left + "px"
            });
        }
    }
	
    $(document).on("click", "input.hasDatepicker + svg", function () {
        $(this).prev().datepicker("show");
    });

    // #endregion

    // #region BOOTSTRAP SELECT CONTROL

    window.createDdlArrowIfRequired = () => {
        for (let $select of $.makeArray($("select")).map(s => $(s))) {
            if (!$select.next(".input-group-append")[0] && !$select.next(".my-ddl-icon")[0]) {
                const right = `${parseFloat($select.parent().css("padding-right")) + parseFloat($select.css("padding-right"))}px`;
                const top = `${$select.position().top}px`;
                const height = `${$select.outerHeight(false)}px`;
                const $icon = $(`<i class='fa fa-chevron-down my-ddl-icon' style='position: absolute; right: ${right}; top: ${top}; height: ${height}'></i>`);
                $icon.insertAfter($select);
            }
        }
    }

    createDdlArrowIfRequired();

    $(document).on("mousedown", "select:not([disabled]), select:not([disabled]) + .input-group-append, select:not([disabled]) + svg", e => {
        e.preventDefault();
        if (e.which !== 1) {
            return false;
        }

        const $select = $(e.target).is("select")
            ? $(e.target)
            : $(e.target).prev("*").is("select")
                ? $(e.target).prev("*")
                : $(e.target).is("path") && $(e.target).closest("svg").prev("select")[0]
                    ? $(e.target).closest("svg").prev("select")
                    : $(e.target).closest("div.input-group-sm, div.input-group-lg, div.input-group").children("select").first();

        let guid = uuidv4();
        if (!$select.attr("guid")) {
            $select.attr("guid", guid);
        } else {
            guid = $select.attr("guid");
        }

        const $inputGroup = $select.closest("div.input-group-sm, div.input-group-lg, div.input-group")[0] ? $select.closest("div.input-group-sm, div.input-group-lg, div.input-group") : null;
        $select.focus();

        const $selectParent = $select.parent();

        let $ulOptionsContainer = $selectParent.children(".my-select-options-container").toArray().map(el => $(el)).filter($el => $el.attr("guid") === $select.attr("guid"))[0] || null;
        const hiding = $ulOptionsContainer !== null;

        if (!$ulOptionsContainer) {
            $ulOptionsContainer = $(`<ul class='my-select-options-container' guid='${guid}'></ul>`);
            const $options = $select.children("option").toArray().map(el => $(el)).filter($el => $el.text().toLowerCase() !== "none");

            const borderRadius = Math.max(...($inputGroup ? $inputGroup.find("span.input-group-text") : $select).css("border-radius").split(" ").map(r => parseFloat(r))) + "px";

            for (let $option of $options) {
                const $liOption = $(`<li class='my-select-option' value='${$option.val()}'>${$option.text()}</li>`);

                $liOption.css({
                    "font-size": $select.css("font-size"),
                    "height": $select.css("height"),
                    "line-height": $select.css("line-height"),
                    "padding": $select.css("padding"),
                    "border-radius": borderRadius
                });
                $ulOptionsContainer.append($liOption);
            }

            const $parent = $select.parent();
            $parent.attr("position", "relative");
            $parent.append($ulOptionsContainer);
            $ulOptionsContainer.css("left", $select.position().left);
            $ulOptionsContainer.css("top", $select.position().top + $select.outerHeight());
            $ulOptionsContainer.css({
                "width": ($inputGroup || $select).outerWidth(false) + "px",
                "display": "none",
                "border-radius": borderRadius
            });

            if ($inputGroup) { // this is to fix bootstrap removing radius on ddl open
                var $prepText = $inputGroup.find(".input-group-prepend span.input-group-text").first();
                var $appText = $inputGroup.find(".input-group-append span.input-group-text").last();

                if ($prepText.length > 0) {
                    $appText.css("border-radius", `${borderRadius} 0 0 ${borderRadius}`);
                }

                if ($appText.length > 0) {
                    $appText.css("border-radius", `0 ${borderRadius} ${borderRadius} 0`);
                }
            }
        }

        $(".my-select-options-container").not($ulOptionsContainer).stop(true, true).animate({
            height: ["hide", "swing"],
            opacity: "hide"
        }, 250, "linear", function () {
            $(this).remove();
        });

        $ulOptionsContainer.stop(true, true).animate({
            height: ["toggle", "swing"],
            opacity: "toggle"
        }, 250, "linear", function () {
            if (hiding) {
                $ulOptionsContainer.remove();
            }
        });
    });

    $(window).on("resize", e => {
        const $selectOptionContainers = $(".my-select-options-container").toArray().map(el => $(el));

        for (let $optionContainer of $selectOptionContainers) {
            const $select = $optionContainer.parent().children("select").toArray().map(el => $(el))
                .filter($el => $el.attr("guid") === $optionContainer.attr("guid"))[0];
            const $inputGroup = $select.closest("div.input-group-sm, div.input-group-lg, div.input-group")[0] ? $select.closest("div.input-group-sm, div.input-group-lg, div.input-group") : null;
            $optionContainer.css({
                "width": ($inputGroup || $select).outerWidth(false) + "px",
                "left": $select.position().left,
                "top": $select.position().top + $select.outerHeight
            });
        }

    });

    $(document).on("mousedown", ".my-select-option", e => {
        if (e.which !== 1) {
            return false;
        }

        const $option = $(e.target);
        const val = $option.attr("value");
        const $ulOptionsContainer = $option.parent();
        const $select = $ulOptionsContainer.parent().children("select").toArray().map(el => $(el))
            .filter($el => $el.attr("guid") === $ulOptionsContainer.attr("guid"))[0];
        const $selectOptions = $select.children("option").toArray().map(el => $(el));

        for (let $selectOption of $selectOptions) {
            if (val === $selectOption.val()) {
                $selectOption.attr("selected", "selected");
            } else {
                $selectOption.removeAttr("selected");
            }
        }

        $ulOptionsContainer.stop(true, true).animate({
            height: ["hide", "swing"],
            opacity: "hide"
        }, 250, "linear", function () {
            $ulOptionsContainer.remove();
        });

        const $inputGroup = $select.closest("div.input-group-sm, div.input-group-lg, div.input-group")[0] ? $select.closest("div.input-group-sm, div.input-group-lg, div.input-group") : null;
        ($inputGroup || $select).focus();

        e.preventDefault();
    });

    // #endregion

    // #region BOOTSTRAP DROPDOWN CONTROL

    $(".dropdown").on("show.bs.dropdown", function () {
        $(this).find(".dropdown-menu").first().stop(true, true).slideDown(250);
    });

    $(".dropdown").on("hide.bs.dropdown", function () {
        $(this).find(".dropdown-menu").first().stop(true, true).slideUp(250);
    });

    // #endregion

    // #region BOOTSTRAP ACCORDION CONTROL

    $(".accordion .card .card-header").on("click", function () {
        const $currCollapse = $(this).closest(".card").find(".collapse");
        const $otherCollapses = $(this).closest(".accordion").find(".collapse").not($currCollapse);

        $currCollapse.collapse("toggle");
        $otherCollapses.collapse("hide");
    });

    $(".accordion .collapse").on("shown.bs.collapse hidden.bs.collapse", function () {
        const $headers = $.makeArray($(this).closest(".accordion").find(".card .collapse").closest(".card").find(".card-header")).map(h => $(h));

        for (let $header of $headers) {
            const isShown = $header.closest(".card").find(".collapse").is(".show");
            const $icons = $.makeArray($header.find("svg")).map(i => $(i));
            const [$iconHidden, $iconShown] = $icons;

            if (isShown) {
                if ($iconShown.hasClass("d-none")) {
                    $iconShown.removeClass("d-none");
                }
                if (!$iconHidden.hasClass("d-none")) {
                    $iconHidden.addClass("d-none");
                }
            } else {
                if (!$iconShown.hasClass("d-none")) {
                    $iconShown.addClass("d-none");
                }
                if ($iconHidden.hasClass("d-none")) {
                    $iconHidden.removeClass("d-none");
                }
            }
        }
    });

    $(".toggle-accordion").on("click", function () {
        const $collapsibleElements = $(this).closest(".card").find(".card-body .accordion .card .collapse");
        const isAnyShown = $collapsibleElements.is(".show");
        if (isAnyShown) {
            const $shown = $collapsibleElements.filter(".show");
            $shown.collapse("hide");
        } else {
            $collapsibleElements.collapse("show");
        }
    });

    $(".show-accordion").on("click", function () {
        const $collapsibleElements = $(this).closest(".card").find(".card-body .accordion .card .collapse");
        const $shown = $collapsibleElements.filter(".show");
        const $hidden = $collapsibleElements.not($shown);
        const isAnyHidden = $hidden.length > 0;
        if (isAnyHidden) {
            $hidden.collapse("show");
        }
    });

    $(".hide-accordion").on("click", function () {
        const $collapsibleElements = $(this).closest(".card").find(".card-body .accordion .card .collapse");
        const isAnyShown = $collapsibleElements.is(".show");
        if (isAnyShown) {
            const $shown = $collapsibleElements.filter(".show");
            $shown.collapse("hide");
        }
    });

    // #endregion

    // #region BOOSTRAP TABS CONTROL

    $(document).on("click", ".nav-tabs .nav-item", function (e) {

        const $currLink = $(this).find("a[data-target]");

        if ($currLink.length === 0) {
            return;
        }

        const $otherLinks = $currLink.closest(".nav-tabs").find("a").not($currLink);

        if ($currLink.is(".active")) {
            return;
        }

        $currLink.addClass("active");
        $otherLinks.removeClass("active");

        const $currTab = $($currLink.attr("data-target"));
        const $otherShownTabs = $($.makeArray($otherLinks).map(l => $(l).attr("data-target")).join(", ")).filter(".show");
        const $allTabs = $($.makeArray($otherLinks).map(l => $(l).attr("data-target")).join(", "));
        $allTabs.push($currTab);

        $otherShownTabs.hide(); // animated with css, working
        $otherShownTabs.removeClass("show active");
        $currTab.show();
        $currTab.addClass("show active");
    });

    // #endregion

    // #region BOOTSTRAP AFFIX PLUGIN

    const $affixes = $.makeArray($("[data-spy='affix']")).map(a => $(a));
    for (let $affix of $affixes) {
        $affix.css({
            "position": "sticky",
            "top": $affix.attr("data-offset-top") + "px"
        });
    }

    // #endregion

    // #region BOOTSTRAP ALERT

    $(document).on("click", ".alert .close", function () {
        $(this).parents(".row").first().hide("fade");
    });

    // #endregion
});