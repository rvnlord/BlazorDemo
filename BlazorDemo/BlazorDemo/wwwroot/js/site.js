window.blazor_EditEmployee_LocationChangedToCurrent = (id) => {
    const $spans = $("span").toArray().map(span => $(span));
    const $editNavLink = $spans.filter($span => $span.text() === "Edit" && $span.parents(".nav-link").length === 1)[0].parents(".nav-link");
    const $createNavLink  = $spans.filter($span => $span.text() === "Create" && $span.parents(".nav-link").length === 1)[0].parents(".nav-link");

    if (id === 0) {
        $createNavLink.parents(".nav-item").siblings(".nav-item").children(".nav-link").removeClass("active");
        $createNavLink.addClass("active");
    } else if (id > 0) {
        $editNavLink.parents(".nav-item").siblings(".nav-item").children(".nav-link").removeClass("active");
        $editNavLink.addClass("active");
    } else if (id === null) {
        $createNavLink.removeClass("active");
        $editNavLink.removeClass("active");
    }
}

window.blazor_EditEmployee_Reinitialized = () => {
    $("input, select").filter(".valid").removeClass("valid");
}