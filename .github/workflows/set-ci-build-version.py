#!/usr/bin/env python3
import os

import gha

def get_environment_variable(name):
    ret = os.getenv(name)

    if ret is None or ret == '':
        gha.print_error(f"Missing required parameter '{name}'")

    return ret

release_tag_name = os.getenv('release_tag_name')
github_run_number = get_environment_variable('github_run_number')
gha.fail_if_errors()

build_version = f'0.0.0-ci{github_run_number}'

if release_tag_name is not None and release_tag_name != '':
    if release_tag_name.startswith('v'):
        release_tag_name = release_tag_name[1:]
    build_version = release_tag_name

gha.set_environment_variable('CiBuildVersion', build_version)
