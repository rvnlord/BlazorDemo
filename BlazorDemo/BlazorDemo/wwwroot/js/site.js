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

function clearValidation() {
    $("input, select").filter(".valid").removeClass("valid");
} 

showAlerts = () => {
    $(".alert:hidden").stop(true, true).fadeIn(250, function() {
        $(this).css("display", "");
    });
}

blazor_EditEmployee_Reinitialized = () => {
    clearValidation();
    showAlerts();
}

blazor_Account_Login_AfterRender = () => {
    clearValidation();
    showAlerts();
}

window.blazor_Account_Login_DisplayAlerts = () => {
    clearValidation();
    showAlerts();
}

blazor_Account_Register_AfterRender = () => {
    clearValidation();
    showAlerts();
}

blazor_Account_ConfirmEmail_AfterRender = () => {
    clearValidation();
    showAlerts();
}

blazor_Account_ForgotPassword_AfterRender = () => {
    clearValidation();
    showAlerts();
}

blazor_Account_ResetPassword_AfterRender = () => {
    clearValidation();
    showAlerts();
}

blazor_Account_ResendEmailConfirmation_AfterRender = () => {
    clearValidation();
    showAlerts();
}

blazor_MainLayout_RefreshLayout = () => {
    clearValidation();
    showAlerts();
}

blazor_Account_Edit_AfterRender = () => {
    clearValidation();
    showAlerts();
}

blazor_Admin_EditUser_AfterRender = () => {
    clearValidation();
    showAlerts();
}

blazor_Admin_AddUser_AfterRender = () => {
    clearValidation();
    showAlerts();
}

blazor_Admin_EditRole_AfterRender = () => {
    clearValidation();
    showAlerts();
}

blazor_Admin_AddRole_AfterRender = () => {
    clearValidation();
    showAlerts();
}

blazor_Admin_EditClaim_AfterRender = () => {
    clearValidation();
    showAlerts();
}

blazor_Admin_AddClaim_AfterRender = () => {
    clearValidation();
    showAlerts();
}

blazor_Employee_EmployeeList_AfterRender = () => {
    clearValidation();
    showAlerts();
}