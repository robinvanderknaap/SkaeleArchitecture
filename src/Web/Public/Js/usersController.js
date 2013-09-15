(function ($, window) {

    "use strict";

    var defaults = {
        selectors: {
            search: '#search',
        },
        urls: {
            GetUsers: ''
        },
        gridOptions: {
            rowHeight: 75,
            editable: false,
            enableAddRow: false,
            enableCellNavigation: false,
            enableColumnReorder: false,
            forceFitColumns: true
        },
        templates: {
            GridCellLayout: ''
        },
        pageSize: 20
    };
    
    var usersController = function (options) {

        this.options = $.extend(defaults, options);

        var self = this;

        this.data = { length: 100 };

        for (var i = 0; i < 100; i++) {
            var d = (this.data[i] = {});

            d["name"] = "User " + i;
            d["email"] = "test.user@nospam.org";
            d["title"] = "Regional sales manager";
            d["phone"] = "206-000-0000";
        }

        var grid = new Slick.Grid("#gridContainer", this.data, this.getColumns(), this.options.gridOptions);

        grid.onViewportChanged.subscribe(function (e, args) {
            var vp = grid.getViewport();
            self.ensureData(vp.top, vp.bottom);
        });

        // If browser window is resized, resize grid canvas. This is not done automatically.
        $(window).bind('resize', function () {
            $("#gridContainer").height($(window).height() - 250);
            grid.resizeCanvas();
        }).trigger('resize');
        
        // load the first page
        grid.onViewportChanged.notify();
    };

    usersController.prototype.getColumns = function () {

        return [{ id: "users", name: "Users", cssClass: "user-cell", formatter: this.renderCell }];
    };

    usersController.prototype.renderCell = function (row, cell, value, columnDef, dataContext) {

        return Mustache.render(options.templates.GridCellLayout, dataContext);
    };
    
    usersController.prototype.ensureData = function (from, to) {

        //// Calculate page which needs to be loaded
        //var fromPage = Math.floor(from / this.options.pageSize);
        //var toPage = Math.floor(to / this.options.pageSize);
        
        //// Calculate first and last row which need to be loaded
        //var firstRow = fromPage * this.options.pageSize;
        //var lastRow = (toPage + 1) * this.options.pageSize;

        //console.log(this.options.pageSize);
        //console.log(fromPage);
        //console.log(toPage);
        //console.log(firstRow);
        //console.log(lastRow);

        //// Remove pages which need not be loaded
        //for (var counter = 0; counter < this.data.length; counter++) {
        //    if (counter < firstRow || counter > lastRow) {
        //        this.data[counter] = undefined;
        //    }
        //}
        




    };

    window.UsersController = usersController;

})($, window);