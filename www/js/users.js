function userController($scope) {
    $scope.fName = '';
    $scope.lName = '';
    $scope.passw1 = '';
    $scope.passw2 = '';
    $scope.newfName = '';
    $scope.newlName = '';
    $scope.newpassw1 = '';
    $scope.newpassw2 = '';
    $scope.users = [
        {
            id: 1,
            fName: 'Hege',
            lName: "Pege"
        },
        {
            id: 2,
            fName: 'Kim',
            lName: "Pim"
        },
        {
            id: 3,
            fName: 'Sal',
            lName: "Smith"
        },
        {
            id: 4,
            fName: 'Jack',
            lName: "Jones"
        },
        {
            id: 5,
            fName: 'John',
            lName: "Doe"
        },
        {
            id: 6,
            fName: 'Peter',
            lName: "Pan"
        }
];
    $scope.edit = true;
    $scope.error = false;
    $scope.incomplete = false;

    $scope.addUser = function () {
        $scope.users.push({
            id: $scope.users.length + 1,
            fName: $scope.newfName,
            lName: $scope.newlName
        });
    };

    function arr_del(d) {
        return $scope.users.slice(0, d - 1).concat($scope.users.slice(d));
    };

    $scope.deleteUser = function (id) {
        $scope.users = arr_del(parseInt(id));
        for (var i = 0; i < $scope.users.length; i++)
            $scope.users[i].id = i + 1;
    };

    $scope.editUser = function (id) {
        if (id == 'new') {
            $scope.edit = true;
            $scope.incomplete = true;
            $scope.fName = '';
            $scope.lName = '';
        } else {
            $scope.edit = false;
            $scope.fName = $scope.users[id - 1].fName;
            $scope.lName = $scope.users[id - 1].lName;
        }
    };

    $scope.$watch('passw1', function () {
        $scope.test();
    });
    $scope.$watch('passw2', function () {
        $scope.test();
    });
    $scope.$watch('fName', function () {
        $scope.test();
    });
    $scope.$watch('lName', function () {
        $scope.test();
    });

    $scope.test = function () {
        if ($scope.passw1 !== $scope.passw2) {
            $scope.error = true;
        } else {
            $scope.error = false;
        }
        $scope.incomplete = false;
        if ($scope.edit && (!$scope.fName.length ||
            !$scope.lName.length ||
            !$scope.passw1.length || !$scope.passw2.length)) {
            $scope.incomplete = true;
        }
    };
}

function apply() {
    var selector = $('#selector').val();
    clearPage();
    if (selector == '') {
        return false;
    } else {
        $('#jquerySelector').text(selector);
        highlight.current = $(selector);
        highlight.current.each(function () {

            var w = $(this).outerWidth();
            var h = $(this).outerHeight();

            var t = $(this).offset().top;
            var l = $(this).offset().left;
            var overlay = $('<span class="highlighted"></span>');
            overlay.width(w).height(h);
            overlay.css({
                top: t,
                left: l
            });
            overlay.appendTo('body');
            overlay.animate({
                backgroundColor: "green"
            }, 1000)
        });
    }
    $('#numElems').text(highlight.current.length);
    return false;
}

define(["jquery", "bootstrap"], function (jQuery) {
    window.swap1 = function() {
        alert("aaa");
        window.document.pic.src = 'images/6.jpg';
    }
});
