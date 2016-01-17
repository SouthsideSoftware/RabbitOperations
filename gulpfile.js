/// <binding Clean='copy-libs' />
"use strict";

var gulp = require('gulp-help')(require('gulp')),
    dnx = require('gulp-dnx');

var paths = {
    src: './src',
    tests: './tests'
};

gulp.task('build', dnx('dnu', {build:true, restore: true, run: false, cwd: paths.src + '/RabbitOperations.Collector'}));

gulp.task('default', ['build']);
