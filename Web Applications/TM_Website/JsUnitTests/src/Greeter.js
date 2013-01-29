//@ typedef */
window.myapp = {};

window.myapp.Greeter = function() { };

window.myapp.Greeter.prototype.greet = function(name)
{
    return name === null ? null : "Hello " + name + "!";
};

window.myapp.Greeter.prototype.greet_forBrowser = function(name)
{
    if ($.browser.msie === undefined)
    {
        return null;
    }
    return "Hello " + name + "!";
};
