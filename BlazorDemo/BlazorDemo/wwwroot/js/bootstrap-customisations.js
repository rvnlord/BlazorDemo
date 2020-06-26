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
        processValidationicons();
        fixSelectMultiples();

        changingDomInProgress = false;
    }

    // #endregion

    // #region BOOTSTRAP WINDOW RESIZES

    $(window).on("resize", e => {

        setEmptyClassForEmptyElements();
        positionDatePickers();

    });

    // #endregion

    // #region LINKS

    $(document).on("click", "a", e => {

        if ($(e.target).closest(":disabled, .disabled, [readonly], .readonly").length > 0) {
            e.preventDefault();
        }

    });

    // #endregion

    // #region BOOTSTRAP GROUPS

    $(document).on("mouseenter", "button, .btn, input, select", e => {
        e.stopPropagation();
        var $hoveredEl = $(e.target).closest("button, .btn, input, select").first(); // span or other element inside can trigger it
        var $groups = $hoveredEl.parents(".input-group, .input-group-append, .input-group-prepend, .btn-group").toArray().map(el => $(el));

        $("button, .btn, input, select").css("z-index", 0);
        const zIndices = $("button, .btn, input, select").toArray().map(el => $(el)).map($el => parseInt($el.css("z-index"))).filter(v => !Number.isNaN(v));
        const zIndex = zIndices.length === 0 ? 0 : Math.max(...zIndices);

        for (let $group of $groups) {
            const $allGroupElements = $group.children().toArray().map(el => $(el));
            const $currentGroupEl = $allGroupElements.filter($el => $el.is($hoveredEl) || $el.find("button, input, select").filter($hoveredEl).length > 0)[0];
            const $otherGroupElements = $allGroupElements.filter($el => !$el.is($currentGroupEl));
           
            for (let $el of $otherGroupElements) {
                $el.css("z-index", zIndex);
            }

            $currentGroupEl.css("z-index", zIndex + 1);
        }

        $(".dropdown.show, .my-select-options-container").parents(".input-group, .input-group-append, .input-group-prepend, .btn-group").css("z-index", zIndex + 2);
    });

    // #endregion

    // #region BOOTSTRAP CHECKBOX

    $(document).on("click", "input[type='checkbox'][readonly]", function(e) {
        e.preventDefault();
    });

    // #endregion

    // #region BOOTSTRAP INPUT

    $(document).on("change", "input.form-control", function() {
        setEmptyClassForEmptyElements();
    });

    window.processValidationicons = () => {
        for (let $validationIcon of $.makeArray($("svg.validation-icon")).map(i => $(i))) {
            if ($validationIcon.attr("position") !== "absolute") {
                const $input = $validationIcon.prev("input");
                const right = `${parseFloat($input.css("padding-right"))}px`;
                const top = `${$input.position().top}px`;
                const height = `${$input.outerHeight(false)}px`;
                $validationIcon.css({
                    "position": "absolute",
                    "right": right,
                    "top": top,
                    "height": height
                });
                $input.css({
                    "border-top-right-radius": "0.25rem",
                    "border-bottom-right-radius": "0.25rem"
                });
            }
        }
    }

    // #endregion

    // #region BOOTSTRAP DATE PICKER

    window.makeDataPickers = () => { // called on dom change
        const $dps = $("input[type='date']").toArray().map(dp => $(dp)).filter($dp => $dp.css("display") !== "none");
        for (let $dp of $dps) {
            $dp.css("display", "none");
            const val = $dp.val().split("-").reverse().join("-");
            const $tdp = $(`<input class='${$dp.attr("class")}' type='text' value='${val}' placeholder='${$dp.attr("placeholder")}' />`);
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
        for (let $select of $.makeArray($("select:not([multiple])")).map(s => $(s))) {
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

    $(document).on("mousedown", "select:not([disabled]):not(.readonly):not([readonly]):not([multiple]), select:not([disabled]):not(.readonly):not([readonly]):not([multiple]) + .input-group-append, select:not([disabled]):not(.readonly):not([readonly]):not([multiple]) + svg", e => {
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

            const borderRadius = Math.max(...($inputGroup ? $inputGroup.find(".fa-chevron-down").parents("button").first() : $select).css("border-radius").split(" ").map(r => parseFloat(r))) + "px";

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

        $(".my-select-options-container").not($ulOptionsContainer).not("select[multiple] + ul.my-select-options-container").stop(true, true).animate({
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
            if ($select.is("select[multiple]")) {
                continue;
            }
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
        if ($select.is("select[multiple]")) {
            return false;
        }
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

    // #region BOOTSTRAP SELECT MULTIPLE CONTROL

    window.fixSelectMultiples = () => { // called on dom change
        const $selectMultiples = $("select[multiple]").toArray().map(s => $(s));
        for (let $selectMultiple of $selectMultiples) {

            let guid = uuidv4();
            if (!$selectMultiple.attr("guid")) {
                $selectMultiple.attr("guid", guid);
            } else {
                guid = $selectMultiple.attr("guid");
            }

            const $selectParent = $selectMultiple.parent();

            let $ulOptionsContainer = $selectParent.children(".my-select-options-container").toArray().map(el => $(el)).filter($el => $el.attr("guid") === $selectMultiple.attr("guid"))[0] || null;

            if (!$ulOptionsContainer) {
                $ulOptionsContainer = $(`<ul class='my-select-options-container' guid='${guid}'></ul>`);
                const $options = $selectMultiple.children("option").toArray().map(el => $(el))
                    .filter($el => $el.text().toLowerCase() !== "none");

                for (let $option of $options) {
                    const $liOption = $(`<li class='my-select-option' value='${$option.val()}'>${$option.text()}</li>`);
                    $ulOptionsContainer.append($liOption);
                }

                $ulOptionsContainer.css({
                    "position": "relative",
                    "max-height": $selectMultiple.outerHeight() + "px",
                    "overflow-y": "scroll",
                    "overflow-x": "hidden",
                    "border-radius": $selectMultiple.css("border-radius"),
                    "-ms-overflow-style": "none", // Internet Explorer 10+
                    "scrollbar-width": "none" // Firefox
                });
                $ulOptionsContainer.addClass("no-scrollbar"); // Chrome
                $selectParent.attr("position", "relative");
                $selectParent.append($ulOptionsContainer);
                $selectMultiple.css("display", "none");

            }

        }
    }

    fixSelectMultiples();

    $(document).on("click", "select[multiple] + ul.my-select-options-container > li.my-select-option", function(e) {
        e.preventDefault();
        if (e.which !== 1) {
            return;
        }

        const $option = $(this);
        const $ulOptionsContainer = $option.parent();
        const $select = $ulOptionsContainer.parent().children("select").toArray().map(el => $(el))
            .filter($el => $el.attr("guid") === $ulOptionsContainer.attr("guid"))[0];
        const $selectOptions = $select.children("option").toArray().map(el => $(el));

        if (!$option.hasClass("selected")) {
            $option.addClass("selected");
        } else {
            $option.removeClass("selected");
        }
        
        for (let $selectOption of $selectOptions) {
            const selectedVals = $ulOptionsContainer.children("li.my-select-option.selected").toArray().map(o => $(o).attr("value"));
            if (selectedVals.some(val => val === $selectOption.val())) {
                $selectOption.attr("selected", "selected");
            } else {
                $selectOption.removeAttr("selected");
            }
        }

        $ulOptionsContainer.addClass("focus");
    });

    $("body").click(function(e) {
        if (!$(e.target).is("select[multiple] + ul.my-select-options-container > li.my-select-option")) {
            $("select[multiple] + ul.my-select-options-container").removeClass("focus");
            //e.stopPropagation(); // don't bubble up if sth other than multi ddl was clicked - which means unfocus only once
        }
        
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
        $(this).parents(".alert").first().hide("fade");
    });

    $(document).on("click", ".btn-alert", function () {
        const $alert = $(this).next(".alert");
        $alert.show("fade");
        setTimeout(function() {
            $alert.hide("fade");
        }, 2000);
    });

    // #endregion

    // #region EXAMPLE COLLAPSIBLE IMAGE GALLERY

    // - OR it can be done with Boostrap alone (take a look at html in 'Index.razor')

    //$("#btnToggle").click(function () {
    //    $("#divImageGallery").collapse("toggle");
    //});

    //$("#btnHide").click(function () {
    //    $("#divImageGallery").collapse("hide");
    //});

    //$("#btnShow").click(function () {
    //    $("#divImageGallery").collapse("show");
    //});

    //$("#divImageGallery").on("show.bs.collapse", function () {
    //    console.log("Image Gallery is about to be expanded");
    //});

    //$("#divImageGallery").on("shown.bs.collapse", function () {
    //    console.log("Image Gallery is expanded");
    //});

    //$("#divImageGallery").on("hide.bs.collapse", function () {
    //    console.log("Image Gallery is about to be collapsed");
    //});

    //$("#divImageGallery").on("hidden.bs.collapse", function () {
    //    console.log("Image Gallery is collapsed");
    //});

    // #endregion

    // #region EXAMPLE MODAL

    //$("#btnShowModal").click(function () {
    //    $("#modalLogin").modal("show");
    //});

    //$("#btnHideModal").click(function () {
    //    $("#modalLogin").modal("hide");
    //});

    //$("#modalLogin").on("show.bs.modal", function () {
    //    console.log("Modal is about to be displayed");
    //});

    //$("#modalLogin").on("shown.bs.modal", function () {
    //    console.log("Modal is displayed");
    //});

    //$("#modalLogin").on("hide.bs.modal", function () {
    //    console.log("Modal is about to be hidden");
    //});

    //$("#modalLogin").on("hidden.bs.modal", function () {
    //    console.log("Modal is hidden");
    //});

    // #endregion

    // #region EXAMPLE BOOTSTRAP TOOLTIP

    //$("button:contains('Tooltip Default')[data-toggle='tooltip']").tooltip({
    //    title: "Tooltip from title option"
    //});

    //$("button:contains('Tooltip Right')[data-toggle='tooltip']").tooltip({
    //    title: "<h3>Help</h3><p>Click to submit the page</p>",
    //    placement: "right",
    //    animation: true,
    //    delay: { show: 500, hide: 500 },
    //    html: true
    //});

    //$("button:contains('Tooltip Click')[data-toggle='tooltip']").tooltip({
    //    title: "Tooltip Click",
    //    trigger: "click"
    //});

    //$("#txtTooltipManual").tooltip({
    //    title: "Tooltip manual",
    //    trigger: "manual",
    //    placement: "bottom",
    //    animation: true
    //});

    //$("#btnInfo").click(function () {
    //    $("#txtTooltipManual").tooltip("toggle");
    //});

    // #endregion

    // #region EXAMPLE BOOTSTRAP POPOVER

    //$("[data-toggle='popover']").popover();

    // #endregion

    // #region EXAMPLE BOOTSTRAP SCROLLSPY

    //var activeListGroupItems = $(".list-group-item.active");
    //$("body").scrollspy({
    //    target: "",
    //    offset: 70
    //});
    //activeListGroupItems.addClass("active"); 

    //window.addEventListener("scroll", function() {
    //    activeListGroupItems.addClass("active"); // readd `active` because scrollspy will remove it for some reason
    //});

    //$(window).on("activate.bs.scrollspy", function () {
    //    activeListGroupItems.addClass("active"); // readd `active` because scrollspy will remove it for some reason
    //});

    //$("a[href*='#']:not([href='#'])").click(function(e) {
    //    e.preventDefault();
    //    $("html, body").stop(true, true).animate({
    //        scrollTop: $($(this).attr("href")).offset().top - 69
    //    }, 250);
    //});

    // #endregion

    // #region EXAMPLE BOOTSTRAP CAROUSEL

    // OR with 'data-attributes'
    //$("#imageCarousel").addClass("slide");
    //$("#imageCarousel").carousel({
    //    interval: 5000,
    //    pause: "hover",
    //    wrap: true
    //});

    //$("#imageCarousel .carousel-control-prev").click(function() {
    //     $("#imageCarousel").carousel("prev");
    //});

    //$("#imageCarousel .carousel-control-next").click(function() {
    //    $("#imageCarousel").carousel("next");
    //});

    // #endregion
});