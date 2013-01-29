var _TestCase = TestCase;

_TestCase("GreeterTest", {
    "test greet": function() {
        var greeter = new myapp.Greeter();
        assertEquals("Hello World!", greeter.greet("World"));
    },
    "test greet null": function() {
        var greeter = new myapp.Greeter();
        assertNull(greeter.greet_forBrowser(null));
    }
});

_TestCase("jQueryTest", {
    "test checkJQuery": function(){
        assertNotUndefined($);
        assertNotUndefined($.browser);
        assertUndefined($.browserAAAA);
    }
});
