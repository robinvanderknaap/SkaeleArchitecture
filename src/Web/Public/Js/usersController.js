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
    
    var usersController = function (remoteModel, options) {

        var self = this;
        self.options = $.extend(defaults, options);
        self.dataRequest = null;

        self.grid = new Slick.Grid("#gridContainer", remoteModel.data, self.getColumns(), self.options.gridOptions);

        self.grid.onViewportChanged.subscribe(function (e, args) {
            var vp = self.grid.getViewport();
            remoteModel.ensureData(vp.top, vp.bottom);
        });

        // If browser window is resized, resize grid canvas. This is not done automatically.
        $(window).bind('resize', function () {
            $("#gridContainer").height($(window).height() - 250);
            self.grid.resizeCanvas();
        }).trigger('resize');
        
        remoteModel.onDataLoaded.subscribe(function (e, args) {
            for (var i = args.from; i <= args.to; i++) {
                self.grid.invalidateRow(i);
            }

            self.grid.updateRowCount();
            self.grid.render();
        });

        // load the first page
        self.grid.onViewportChanged.notify();
    };

    usersController.prototype.getColumns = function () {

        return [{ id: "users", name: "Users", cssClass: "user-cell", formatter: this.renderCell }];
    };

    usersController.prototype.renderCell = function (row, cell, value, columnDef, dataContext) {

        return Mustache.render(options.templates.GridCellLayout, dataContext);
    };
    
    window.UsersController = usersController;

})($, window);