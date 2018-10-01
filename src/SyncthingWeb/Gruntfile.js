/// <binding AfterBuild='build' Clean='clean' />
module.exports = function (grunt) {
    grunt.initConfig({
        clean: ["wwwroot/lib/*", "temp/"],
        copy: {
            main: {
                files: [
                  { expand: true, cwd: "bower_components/bootstrap/dist", src: ['**'], dest: 'wwwroot/lib/bootstrap/dist/' },
                  { expand: true, cwd: "bower_components/jquery/dist", src: ['**'], dest: 'wwwroot/lib/jquery/dist/' },
                  { expand: true, cwd: "bower_components/jquery-validation/dist", src: ['**'], dest: 'wwwroot/lib/jquery-validation/dist/' },
                  { expand: true, cwd: "bower_components/jquery-validation-unobtrusive/", src: ['*.js'], dest: 'wwwroot/lib/jquery-validation-unobtrusive/' },
                    { expand: true, cwd: "bower_components/select2/dist", src: ['**'], dest: 'wwwroot/lib/select2/dist/' },
                    { expand: true, cwd: "bower_components/moment/min", src: ['**'], dest: 'wwwroot/lib/momentjs/dist/' },
                    { expand: true, cwd: "bower_components/AdminLTE/dist", src: ['**'], dest: 'wwwroot/lib/AdminLTE/dist/' }
                ],
            },
        }
    });

    grunt.loadNpmTasks("grunt-contrib-clean");
    grunt.loadNpmTasks("grunt-contrib-copy");

    grunt.registerTask("build", ["clean", 'copy']);
};