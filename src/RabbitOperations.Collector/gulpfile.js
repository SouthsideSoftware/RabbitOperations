/// <binding Clean='copy-libs' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    bower = require("gulp-bower");

var paths = {
    webroot: "./wwwroot/",
    bower: "./bower_modules/"
};

paths.lib = "./" + paths.webroot + "/lib/";
paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task('clean:lib', function(cb) {
    rimraf(paths.lib, cb);
});

gulp.task('clean', ['clean:js', 'clean:css', 'clean:lib']);

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("copy-libs", ['clean', 'bower'], function () {
    var bower = {
        "bootstrap": "bootstrap/dist/**/*.{js,map,css,ttf,svg,woff,woff2,eot}",
        "jquery": "jquery/dist/jquery*.{js,map}",
        "jquery-validation": "jquery-validation/dist/jquery.validate*.{js,map}",
        "jquery-validation-unobtrusive": "jquery-validation-unobtrusive/jquery.validate.unobtrusive*.{js,map}",
        "angular": "angular/angular*.{js,map}",
        "signalr": "signalr/jquery.signalR*.{js,map}",
        'angular-bootstrap': 'angular-bootstrap/ui-bootstrap*.{js,map}',
        'noty': 'noty/js/noty/jquery.noty*.{js,map}',
        'noty/themes': 'noty/js/noty/themes/*',
        'angular-highlightjs': 'angular-highlightjs/build/angular-highlightjs*.{js,map}',
        'moment': 'moment/min/moment*{js,map}',
        'showdown': 'showdown/dist/showdown*.{js,map}'
    }

    for (var destinationDir in bower) {
        gulp.src(paths.bower + bower[destinationDir])
          .pipe(gulp.dest(paths.lib + destinationDir));
    }
});

gulp.task('bower-prune', function () {
    return bower({cmd: 'prune'});
});

gulp.task('bower', ['bower-prune'], function () {
    return bower();
});

gulp.task("min", ["min:js", "min:css"]);
