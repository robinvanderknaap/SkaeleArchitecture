(function ($, window) {

    "use strict";

    var defaults = {
        pageSize: 50
    };
    
    // Private vars
    var searchstr = "";
    var sortcol = null;
    var sortdir = 1;
    var hRequest = null;
    var req = null; // ajax request

    

    var usersRemoteModel = function (options) {

        var self = this;
        self.options = $.extend(defaults, options);

        this.data = { length: 0 };
        
        // Events
        self.onDataLoading = new Slick.Event();
        self.onDataLoaded = new Slick.Event();
    };

    usersRemoteModel.prototype.clear = function () {
        for (var key in this.data) {
            delete this.data[key];
        }
        this.data.length = 0;
    };

    usersRemoteModel.prototype.isDataLoaded = function (from, to) {
        for (var i = from; i <= to; i++) {
            if (this.data[i] == undefined || this.data[i] == null) {
                return false;
            }
        }
        return true;
    };

    usersRemoteModel.prototype.ensureData = function (from, to) {

        var self = this;

        if (req) {
            req.abort();
            for (var i = req.fromPage; i <= req.toPage; i++) {
                self.data[i * self.options.pageSize] = undefined;
            }
        }

        if (from < 0) {
            from = 0;
        }

        if (self.data.length > 0) {
            to = Math.min(to, self.data.length - 1);
        }

        var fromPage = Math.floor(from / self.options.pageSize);
        var toPage = Math.floor(to / self.options.pageSize);

        while (self.data[fromPage * self.options.pageSize] !== undefined && fromPage < toPage) {
            fromPage++;
        }

        while (self.data[toPage * self.options.pageSize] !== undefined && fromPage < toPage) {
            toPage--;
        }

        if (fromPage > toPage || ((fromPage == toPage) && self.data[fromPage * self.options.pageSize] !== undefined)) {
            // TODO:  look-ahead
            onDataLoaded.notify({ from: from, to: to });
            return;
        }
        var url = '/User/GetUsers?toPage=' + toPage + '&fromPage=' + fromPage + '&pageSize=' + this.options.pageSize;

        //if (sortcol != null) {
        //    url += ("&sortby=" + sortcol + ((sortdir > 0) ? "+asc" : "+desc"));
        //}

        if (hRequest != null) {
            clearTimeout(hRequest);
        }
        
        hRequest = setTimeout(function () {
            for (var p = fromPage; p <= toPage; p++) {
                self.data[p * self.options.pageSize] = null; // null indicates a 'requested but not available yet'
            }

            self.onDataLoading.notify({ from: from, to: to });

            req = $.ajax({
                url: url,
                dataType: 'json',
                callbackParameter: "callback",
                cache: true,
                success: function(resp) {
                    console.log(resp);
                    self.data.length = resp.TotalUsers;

                    for (var i = 0; i < 50; i++) {
                        var item = resp.UserViewModels[i];

                        self.data[from + i] = item;
                        self.data[from + i].index = from + i;
                    }

                    req = null;

                    self.onDataLoaded.notify({ from: from, to: to });
                },
                error: function () {
                    onError(fromPage, toPage);
                }
            });

            req.fromPage = fromPage;
            req.toPage = toPage;
        }, 50);
    };
    
    function onError(fromPage, toPage) {
        alert("error loading pages " + fromPage + " to " + toPage);
    }

    function onSuccess(resp) {
        console.log(resp);
        var self = this;

        var from = 0, to = from + 50;
        self.data.length = resp.TotalUsers;

        for (var i = 0; i < 50; i++) {
            var item = resp.UserViewModels[i];

            self.data[from + i] = item;
            self.data[from + i].index = from + i;
        }

        req = null;

        onDataLoaded.notify({ from: from, to: to });
    }


    usersRemoteModel.prototype.reloadData = function (from, to) {
        for (var i = from; i <= to; i++)
            delete this.data[i];

        this.ensureData(from, to);
    };


    usersRemoteModel.prototype.setSort = function (column, dir) {
        sortcol = column;
        sortdir = dir;
        this.clear();
    };

    usersRemoteModel.prototype.setSearch = function (str) {
        searchstr = str;
        this.clear();
    };

    window.UsersRemoteModel = usersRemoteModel;

})($, window);